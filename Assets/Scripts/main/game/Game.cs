using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Threading.Tasks;
using gamecore.action;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using gamecore.game.state;
using UnityEngine;
using UnityEngine.Android;

namespace gamecore.game
{
    public interface IGame
    {
        IPlayer Player1 { get; }
        IPlayer Player2 { get; }
        Dictionary<IPlayer, List<List<ICard>>> Mulligans { get; }
        int TurnCounter { get; }
    }

    internal class Game : IGame, IActionPerformer<EndTurnGA>
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

        public GameSetupBuilder GameSetupBuilder { get; private set; }
        public IGameState GameState { get; set; }
        public Dictionary<IPlayer, List<List<ICard>>> Mulligans
        {
            get
            {
                var result = new Dictionary<IPlayer, List<List<ICard>>>();
                foreach (var pair in GameSetupBuilder.Mulligans)
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
        private readonly ActionSystem _actionSystem = ActionSystem.INSTANCE;

        public Game(IPlayerLogic player1, IPlayerLogic player2)
        {
            Player1 = player1;
            Player2 = player2;
            _actionSystem.AttachPerformer<EndTurnGA>(this);
            CardSystem.INSTANCE.Enable();
            DamageSystem.INSTANCE.Enable();
            GeneralMechnicSystem.INSTANCE.Enable(this);
            GameState = new CreatedState();
            _players.Add(player1);
            _players.Add(player2);
        }

        public async Task PerformSetup()
        {
            GameSetupBuilder = new GameSetupBuilder().WithPlayer1(Player1).WithPlayer2(Player2);
            await GameSetupBuilder.Setup();
            await AdvanceGameState();
        }

        public async Task StartGame()
        {
            TurnCounter++;
            Player1.IsActive = true;
            await _actionSystem.Perform(new DrawCardGA(1, Player1));
        }

        public async Task EndTurn()
        {
            var endTurn = new EndTurnGA();
            await _actionSystem.Perform(endTurn);
            await AdvanceGameState();
        }

        public Task<EndTurnGA> Perform(EndTurnGA endTurnGA)
        {
            if (Player1.IsActive)
            {
                Player1.IsActive = false;
                Player2.IsActive = true;
                endTurnGA.NextPlayer = Player2;
            }
            else
            {
                Player2.IsActive = false;
                Player1.IsActive = true;
                endTurnGA.NextPlayer = Player1;
            }
            TurnCounter++;
            return Task.FromResult(endTurnGA);
        }

        public void EndGame(IPlayerLogic winner)
        {
            _actionSystem.DetachPerformer<EndTurnGA>();
            CardSystem.INSTANCE.Disable();
            DamageSystem.INSTANCE.Disable();
            GeneralMechnicSystem.INSTANCE.Disable();
            GameState = new GameOverState(winner);
            AwaitGeneralInteraction();
        }

        /* Returns if both active Pokemon are set */
        internal async Task SetActivePokemon(ICardLogic basicPokemon)
        {
            await basicPokemon.Play();
            await AdvanceGameState();
        }

        public async Task AdvanceGameState()
        {
            GameState = GameState.AdvanceSuccesfully();
            await GameState.OnAdvanced(this);
        }

        internal async Task PlayCard(ICardLogic card)
        {
            await card.Play();
            await AdvanceGameState();
        }

        internal void AwaitInteraction()
        {
            AwaitInteractionEvent?.Invoke();
        }

        internal void AwaitGeneralInteraction()
        {
            AwaitGeneralInteractionEvent?.Invoke();
        }

        internal async Task<List<ICardLogic>> AwaitSelection(
            IPlayerLogic player,
            List<ICardLogic> options,
            int amount
        )
        {
            var tcs = new TaskCompletionSource<List<ICardLogic>>();
            GameState = new WaitForInputState(tcs, player, options, amount);
            AwaitInteraction();
            return await tcs.Task;
        }

        internal async Task DrawMulliganCards(int numberOfExtraCards, IPlayerLogic player)
        {
            await _actionSystem.Perform(new DrawCardGA(numberOfExtraCards, player));
            await AdvanceGameState();
        }

        internal void SetPrizeCards()
        {
            foreach (var player in _players)
            {
                player.SetPrizeCards();
            }
        }

        internal async Task PlayCardWithTargets(ICardLogic card, List<ICardLogic> targets)
        {
            await card.PlayWithTargets(targets);
            await AdvanceGameState();
        }

        internal async Task PerformAttack(IAttackLogic attack, IPokemonCardLogic attacker)
        {
            await _actionSystem.Perform(new AttackGA(attack, attacker));
            await AdvanceGameState();
        }

        internal async Task Retreat(
            IPokemonCardLogic pokemon,
            List<IEnergyCardLogic> energyCardsToDiscard
        )
        {
            await _actionSystem.Perform(new RetreatGA(pokemon, energyCardsToDiscard));
            await AdvanceGameState();
        }
    }
}
