using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.common;
using gamecore.effect;
using gamecore.game.action;

namespace gamecore.game
{
    public interface IGameController
    {
        IGame Game { get; }
        event EventHandler<List<GameInteraction>> NotifyPlayer1;
        event EventHandler<List<GameInteraction>> NotifyPlayer2;
        event EventHandler<List<GameInteraction>> NotifyGeneral;
        static IGameController Create(string logFilePath)
        {
            return new GameController(logFilePath);
        }

        Task CreateGame(
            Dictionary<string, int> deckList1,
            Dictionary<string, int> deckList2,
            string player1Name,
            string player2Name
        );
        Task RecreateGameFromLog();
        Task StartReplay();
        void StartGame();
        GameStateJson ExportGameState(string playerName);
    }

    class GameController : IGameController, IActionPerformer<CreateGameGA>
    {
        private Game _game;

        public event EventHandler<List<GameInteraction>> NotifyPlayer1;
        public event EventHandler<List<GameInteraction>> NotifyPlayer2;
        public event EventHandler<List<GameInteraction>> NotifyGeneral;
        private readonly ActionSystem _actionSystem;

        public IGame Game => _game;

        public GameController(string logFilePath)
        {
            _actionSystem = new ActionSystem(logFilePath);
            _actionSystem.AttachPerformer<CreateGameGA>(this);
        }

        private void NotifyPlayers()
        {
            if (_game.IsReplaying)
            {
                HandleReplayMode();
                return;
            }
            var interactions = _game.GameState.GetGameInteractions(this, _game.Player1);
            if (interactions.Count > 0)
            {
                NotifyPlayer1?.Invoke(this, interactions);
            }
            else
            {
                interactions = _game.GameState.GetGameInteractions(this, _game.Player2);
                if (interactions.Count > 0)
                {
                    NotifyPlayer2?.Invoke(this, interactions);
                }
                else
                {
                    throw new IlleagalStateException("No interactions found for players");
                }
            }
        }

        protected virtual void OnExpectGeneralInteraction()
        {
            if (_game.IsReplaying)
            {
                HandleReplayMode();
                return;
            }
            var interactions = _game.GameState.GetGameInteractions(this, null);
            if (interactions.Count > 0)
                NotifyGeneral?.Invoke(this, interactions);
        }

        private void HandleReplayMode()
        {
            var replayInteraction = new List<GameInteraction>()
            {
                new(async () => await ReplayNextAction(), GameInteractionType.ReplayNextAction),
            };
            NotifyGeneral?.Invoke(this, replayInteraction);
        }

        private async Task ReplayNextAction()
        {
            var hasMoreActions = await _actionSystem.ReplayNextAction();
            _game.IsReplaying = hasMoreActions;
            _game.AdvanceGameState();
        }

        public async Task CreateGame(
            Dictionary<string, int> deckList1,
            Dictionary<string, int> deckList2,
            string player1Name,
            string player2Name
        )
        {
            await _actionSystem.Perform(
                new CreateGameGA(deckList1, deckList2, player1Name, player2Name)
            );
        }

        public async Task RecreateGameFromLog()
        {
            await _actionSystem.RecreateGameStateFromLog();
        }

        public async Task StartReplay()
        {
            var hasMoreActions = await _actionSystem.ReplayNextAction();
            _game.IsReplaying = hasMoreActions;
        }

        public void StartGame()
        {
            _game.GameState.OnAdvanced(_game);
        }

        public async Task SelectActivePokemon(IPokemonCardLogic basicPokemon)
        {
            await _actionSystem.Perform(new SetActivePokemonGA(basicPokemon));
            _game.AdvanceGameState();
        }

        public async Task PlayCard(ICardLogic card)
        {
            await _actionSystem.Perform(new PlayCardGA(card));
            _game.AdvanceGameState();
        }

        public async Task PlayCardWithTargets(ICardLogic card, List<ICardLogic> targets)
        {
            await _actionSystem.Perform(new PlayCardGA(card, targets));
            _game.AdvanceGameState();
        }

        public async Task SelectMulligans(int numberOfExtraCards, IPlayerLogic player)
        {
            await _actionSystem.Perform(new DrawMulliganCardsGA(numberOfExtraCards, player));
            _game.AdvanceGameState();
        }

        public async Task PerformAttack(IAttackLogic attack, IPokemonCardLogic attacker)
        {
            await _actionSystem.Perform(new AttackGA(attack, attacker));
            _game.AdvanceGameState();
        }

        internal async Task EndTurn()
        {
            await _actionSystem.Perform(new EndTurnGA());
            _game.AdvanceGameState();
        }

        internal void Confirm()
        {
            _game.AdvanceGameState();
        }

        internal async Task Retreat(
            IPokemonCardLogic pokemon,
            List<IEnergyCardLogic> energyCardsToDiscard
        )
        {
            await _actionSystem.Perform(new RetreatGA(pokemon, energyCardsToDiscard));
            _game.AdvanceGameState();
        }

        internal async Task PerformAbility(IPokemonCardLogic pokemon)
        {
            await _actionSystem.Perform(new PerformAbilityGA(pokemon));
            _game.AdvanceGameState();
        }

        public Task<CreateGameGA> Perform(CreateGameGA action)
        {
            return CreateGame(action);
        }

        public Task<CreateGameGA> Reperform(CreateGameGA action)
        {
            return CreateGame(action);
        }

        private Task<CreateGameGA> CreateGame(CreateGameGA action)
        {
            _game = new GameBuilder()
                .WithPlayer1(action.Player1Name)
                .WithPlayer2(action.Player2Name)
                .WithPlayer1Decklist(action.DeckList1)
                .WithPlayer2Decklist(action.DeckList2)
                .WithActionSystem(_actionSystem)
                .Build();
            _game.AwaitInteractionEvent += NotifyPlayers;
            _game.AwaitGeneralInteractionEvent += OnExpectGeneralInteraction;
            return Task.FromResult(action);
        }

        internal async Task SetPrizeCards()
        {
            await _actionSystem.Perform(new SetPrizeCardsGA());
            _game.AdvanceGameState();
        }

        internal async Task StartFirstTurnOfGame()
        {
            await _actionSystem.Perform(new StartTurnGA(_game.Player1));
            _game.AdvanceGameState();
        }

        public GameStateJson ExportGameState(string playerName)
        {
            if (_game == null)
                throw new InvalidOperationException("Game has not been created yet.");

            var player = _game.GetPlayerByName(playerName);
            PlayerStateJson selfState;
            PlayerStateJson opponentState;
            if (player == _game.Player1)
            {
                selfState = _game.Player1.ToSerializable();
                opponentState = _game.Player2.ToSerializable();
            }
            else
            {
                selfState = _game.Player2.ToSerializable();
                opponentState = _game.Player1.ToSerializable();
            }
            var cardStates = CardStateCreator.CreateCardStates(player);
            if (cardStates.Count != 120)
            {
                throw new IlleagalStateException("Card states count is not 120");
            }

            return new GameStateJson(selfState, opponentState, cardStates);
        }
    }
}
