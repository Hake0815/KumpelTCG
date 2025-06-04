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
        private readonly ActionSystem _actionSystem = ActionSystem.INSTANCE;

        public Game(IPlayerLogic player1, IPlayerLogic player2)
        {
            Player1 = player1;
            Player2 = player2;
            _actionSystem.AttachPerformer<EndTurnGA>(this);
            _actionSystem.AttachPerformer<StartTurnGA>(this);
            _actionSystem.AttachPerformer<SetupGA>(this);
            _actionSystem.AttachPerformer<SetPrizeCardsGA>(this);
            CardSystem.INSTANCE.Enable(this);
            DamageSystem.INSTANCE.Enable();
            GeneralMechnicSystem.INSTANCE.Enable(this);
            GameState = new CreatedState();
            _players.Add(player1);
            _players.Add(player2);
        }

        public async Task StartGame()
        {
            await _actionSystem.Perform(new StartTurnGA(Player1));
        }

        public Task<EndTurnGA> Perform(EndTurnGA endTurnGA)
        {
            if (Player1.IsActive)
            {
                Player1.IsActive = false;
                ActionSystem.INSTANCE.AddReaction(new StartTurnGA(Player2));
            }
            else
            {
                Player2.IsActive = false;
                ActionSystem.INSTANCE.AddReaction(new StartTurnGA(Player1));
            }
            return Task.FromResult(endTurnGA);
        }

        public void EndGame(IPlayerLogic winner)
        {
            _actionSystem.DetachPerformer<EndTurnGA>();
            _actionSystem.DetachPerformer<StartTurnGA>();
            _actionSystem.DetachPerformer<SetupGA>();
            _actionSystem.DetachPerformer<SetPrizeCardsGA>();
            CardSystem.INSTANCE.Disable();
            DamageSystem.INSTANCE.Disable();
            GeneralMechnicSystem.INSTANCE.Disable();
            GameState = new GameOverState(winner);
            AwaitGeneralInteraction();
        }

        public async Task AdvanceGameState()
        {
            GameState = GameState.AdvanceSuccesfully();
            await GameState.OnAdvanced(this);
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
            int amount,
            SelectFrom selectFrom
        )
        {
            var tcs = new TaskCompletionSource<List<ICardLogic>>();
            GameState = new WaitForInputState(tcs, player, options, amount, selectFrom);
            AwaitInteraction();
            return await tcs.Task;
        }

        public Task<StartTurnGA> Perform(StartTurnGA action)
        {
            action.NextPlayer.IsActive = true;
            action.NextPlayer.TurnCounter++;
            TurnCounter++;
            return Task.FromResult(action);
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
            action.PlayerHands = new Dictionary<string, IHandLogic>
            {
                { Player1.Name, Player1.Hand },
                { Player2.Name, Player2.Hand },
            };
            return Task.FromResult(action);
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

        // internal void SetPrizeCards() { }

        // public async Task PerformSetup()
        // {
        //     GameSetupBuilder = new GameSetupBuilder().WithPlayer1(Player1).WithPlayer2(Player2);
        //     GameSetupBuilder.Setup();
        //     await AdvanceGameState();
        // }
    }
}
