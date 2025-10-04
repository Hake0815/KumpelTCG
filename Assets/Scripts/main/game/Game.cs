using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamecore.action;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.common;
using gamecore.effect;
using gamecore.game.action;
using gamecore.game.state;
using UnityEngine;

namespace gamecore.game
{
    public interface IGame
    {
        IPlayer Player1 { get; }
        IPlayer Player2 { get; }
        Dictionary<IPlayer, List<List<ICard>>> Mulligans { get; }
        int TurnCounter { get; }
    }

    class Game
        : IGame,
            IActionPerformer<EndTurnGA>,
            IActionPerformer<StartTurnGA>,
            IActionPerformer<SetupGA>,
            IActionPerformer<SetPrizeCardsGA>
    {
        public IPlayerLogic Player1 { get; private set; }
        public IPlayerLogic Player2 { get; private set; }
        IPlayer IGame.Player1
        {
            get => Player1;
        }
        IPlayer IGame.Player2
        {
            get => Player2;
        }
        private readonly List<IPlayerLogic> _players = new();
        public IGameState GameState { get; set; }
        private Dictionary<IPlayerLogic, List<List<ICardLogic>>> _mulligans;
        public Dictionary<IPlayer, List<List<ICard>>> Mulligans
        {
            get
            {
                var result = new Dictionary<IPlayer, List<List<ICard>>>();
                foreach (var pair in _mulligans)
                {
                    var outerList = new List<List<ICard>>();
                    foreach (var innerList in pair.Value)
                    {
                        outerList.Add(innerList.Cast<ICard>().ToList());
                    }
                    result[pair.Key] = outerList;
                }
                return result;
            }
        }
        public int TurnCounter { get; private set; } = 0;
        public event Action AwaitInteractionEvent;
        public event Action AwaitGeneralInteractionEvent;
        private readonly ActionSystem _actionSystem;
        private readonly CardSystem _cardSystem;
        private readonly DamageSystem _damageSystem;
        private readonly GeneralMechnicSystem _generalMechnicSystem;

        public Game(IPlayerLogic player1, IPlayerLogic player2, ActionSystem actionSystem)
        {
            Player1 = player1;
            Player2 = player2;
            _actionSystem = actionSystem;
            _actionSystem.AttachPerformer<EndTurnGA>(this);
            _actionSystem.AttachPerformer<StartTurnGA>(this);
            _actionSystem.AttachPerformer<SetupGA>(this);
            _actionSystem.AttachPerformer<SetPrizeCardsGA>(this);
            _cardSystem = new CardSystem(_actionSystem, this);
            _damageSystem = new DamageSystem(_actionSystem, this);
            _generalMechnicSystem = new GeneralMechnicSystem(_actionSystem, this);
            _players.Add(player1);
            _players.Add(player2);
        }

        public void EndGame(IPlayerLogic winner)
        {
            _actionSystem.DetachPerformer<EndTurnGA>();
            _actionSystem.DetachPerformer<StartTurnGA>();
            _actionSystem.DetachPerformer<SetupGA>();
            _actionSystem.DetachPerformer<SetPrizeCardsGA>();
            _cardSystem.Disable();
            _damageSystem.Disable();
            _generalMechnicSystem.Disable();
            GameState = new GameOverState(winner);
            AwaitGeneralInteraction();
        }

        public void AdvanceGameState()
        {
            GameState = GameState.AdvanceSuccesfully();
            GameState.OnAdvanced(this);
        }

        public void AdvanceGameStateQuietly()
        {
            GameState = GameState.AdvanceSuccesfully();
        }

        internal void AwaitInteraction()
        {
            AwaitInteractionEvent?.Invoke();
        }

        internal void AwaitGeneralInteraction()
        {
            AwaitGeneralInteractionEvent?.Invoke();
        }

        public async Task<List<ICardLogic>> AwaitSelection(
            IPlayerLogic player,
            List<ICardLogic> options,
            Predicate<List<ICard>> selectionCondition,
            bool isQuickSelection,
            SelectFrom selectFrom
        )
        {
            var tcs = new TaskCompletionSource<List<ICardLogic>>();
            GameState = new WaitForInputState(
                tcs,
                player,
                options,
                selectionCondition,
                selectFrom,
                isQuickSelection
            );
            AwaitInteraction();
            return await tcs.Task;
        }

        public Task<EndTurnGA> Perform(EndTurnGA endTurnGA)
        {
            if (Player1.IsActive)
            {
                Player1.IsActive = false;
                endTurnGA.NextPlayer = Player2;
            }
            else
            {
                Player2.IsActive = false;
                endTurnGA.NextPlayer = Player1;
            }
            _actionSystem.AddReaction(new StartTurnGA(endTurnGA.NextPlayer));
            return Task.FromResult(endTurnGA);
        }

        public Task<EndTurnGA> Reperform(EndTurnGA endTurnGA)
        {
            Player1.IsActive = false;
            Player2.IsActive = false;
            return Task.FromResult(endTurnGA);
        }

        public Task<StartTurnGA> Perform(StartTurnGA action)
        {
            StartTurnForPlayer(action.NextPlayer);
            return Task.FromResult(action);
        }

        public Task<StartTurnGA> Reperform(StartTurnGA action)
        {
            StartTurnForPlayer(GetPlayerByName(action.NextPlayer.Name));
            GameState = new IdlePlayerTurnState();
            return Task.FromResult(action);
        }

        private void StartTurnForPlayer(IPlayerLogic nextPlayer)
        {
            if (nextPlayer.Deck.Cards.Count == 0)
            {
                EndGame(nextPlayer.Opponent);
            }
            nextPlayer.IsActive = true;
            nextPlayer.TurnCounter++;
            TurnCounter++;
            if (TurnCounter == 1)
                nextPlayer.AddEffect(FirstTurnOfGameEffect.Create(_actionSystem));
        }

        public Task<SetupGA> Perform(SetupGA action)
        {
            var gameSetupBuilder = new GameSetupBuilder().WithPlayer1(Player1).WithPlayer2(Player2);
            gameSetupBuilder.Setup();
            _mulligans = gameSetupBuilder.Mulligans;
            action.Mulligans = new Dictionary<string, List<List<ICardLogic>>>
            {
                { Player1.Name, gameSetupBuilder.GetMulligansForPlayer(Player1) },
                { Player2.Name, gameSetupBuilder.GetMulligansForPlayer(Player2) },
            };
            action.PlayerHands = new Dictionary<string, List<ICardLogic>>
            {
                { Player1.Name, Player1.Hand.Cards },
                { Player2.Name, Player2.Hand.Cards },
            };
            GameState = new SetupCompletedState();
            return Task.FromResult(action);
        }

        public Task<SetupGA> Reperform(SetupGA action)
        {
            _mulligans = RecreateMulligans(action.Mulligans);
            foreach (var player in _players)
            {
                player.Deck.Shuffle();
                var handCards = player.DeckList.GetCardsByDeckIds(action.PlayerHands[player.Name]);
                player.Hand.AddCards(handCards);
                player.Deck.RemoveCards(handCards);
            }
            GameState = new SettingActivePokemonState();
            return Task.FromResult(action);
        }

        private Dictionary<IPlayerLogic, List<List<ICardLogic>>> RecreateMulligans(
            Dictionary<string, List<List<ICardLogic>>> loggedMulligans
        )
        {
            var result = new Dictionary<IPlayerLogic, List<List<ICardLogic>>>();
            foreach (var pair in loggedMulligans)
            {
                var player = GetPlayerByName(pair.Key);
                var outerList = new List<List<ICardLogic>>();
                foreach (var innerList in pair.Value)
                {
                    outerList.Add(player.DeckList.GetCardsByDeckIds(innerList));
                }
                result[player] = outerList;
            }
            return result;
        }

        public IPlayerLogic GetPlayerByName(string name)
        {
            return name == Player1.Name ? Player1 : Player2;
        }

        public Task<SetPrizeCardsGA> Perform(SetPrizeCardsGA action)
        {
            foreach (var player in _players)
            {
                player.SetPrizeCards();
            }
            action.PrizeCards = new Dictionary<string, List<ICardLogic>>
            {
                { Player1.Name, Player1.Prizes.Cards },
                { Player2.Name, Player2.Prizes.Cards },
            };
            return Task.FromResult(action);
        }

        public Task<SetPrizeCardsGA> Reperform(SetPrizeCardsGA action)
        {
            foreach (var player in _players)
            {
                var cards = player.DeckList.GetCardsByDeckIds(action.PrizeCards[player.Name]);
                player.Deck.RemoveFaceDown(cards);
                player.Prizes.AddCards(cards);
                player.Prizes.Shuffle();
            }
            AdvanceGameStateQuietly();
            return Task.FromResult(action);
        }

        public List<ICardLogic> FindCardsAnywhere(List<ICardLogic> cards)
        {
            var result = new List<ICardLogic>();
            foreach (var card in cards)
            {
                result.Add(FindCardAnywhere(card));
            }
            return result;
        }

        public ICardLogic FindCardAnywhere(ICardLogic card)
        {
            var owner = GetPlayerByName(card.Owner.Name);
            var deckId = card.DeckId;

            var cardReference = owner.DeckList.GetCardByDeckId(deckId);
            if (cardReference != null)
                return cardReference;
            throw new IlleagalStateException($"Could not find card {card} for Player {owner}!");
        }
    }
}
