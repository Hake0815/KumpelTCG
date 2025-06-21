using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using UnityEngine;

namespace gamecore.game
{
    public interface IGameController
    {
        IGame Game { get; }
        event EventHandler<List<GameInteraction>> NotifyPlayer1;
        event EventHandler<List<GameInteraction>> NotifyPlayer2;
        event EventHandler<List<GameInteraction>> NotifyGeneral;
        static IGameController Create()
        {
            return new GameController();
        }

        Task CreateGame(
            Dictionary<string, int> deckList1,
            Dictionary<string, int> deckList2,
            string player1Name,
            string player2Name,
            string logFilePath
        );
        Task RecreateGameFromLog(string logFilePath);
        void StartGame();
    }

    class GameController : IGameController, IActionPerformer<CreateGameGA>
    {
        private Game _game;

        public event EventHandler<List<GameInteraction>> NotifyPlayer1;
        public event EventHandler<List<GameInteraction>> NotifyPlayer2;
        public event EventHandler<List<GameInteraction>> NotifyGeneral;
        private readonly ActionSystem _actionSystem = ActionSystem.INSTANCE;

        public IGame Game
        {
            get { return _game; }
        }

        public GameController(Game game)
        {
            _game = game;
            _game.AwaitInteractionEvent += NotifyPlayers;
            _game.AwaitGeneralInteractionEvent += OnExpectGeneralInteraction;
        }

        public GameController()
        {
            _actionSystem.AttachPerformer<CreateGameGA>(this);
        }

        private void NotifyPlayers()
        {
            OnNotifyPlayer1();
            OnNotifyPlayer2();
        }

        protected virtual void OnNotifyPlayer1()
        {
            var interactions = _game.GameState.GetGameInteractions(this, _game.Player1);
            if (interactions.Count > 0)
            {
                NotifyPlayer1?.Invoke(this, interactions);
            }
        }

        protected virtual void OnNotifyPlayer2()
        {
            var interactions = _game.GameState.GetGameInteractions(this, _game.Player2);
            if (interactions.Count > 0)
            {
                NotifyPlayer2?.Invoke(this, interactions);
            }
        }

        protected virtual void OnExpectGeneralInteraction()
        {
            var interactions = _game.GameState.GetGameInteractions(this, null);
            if (interactions.Count > 0)
                NotifyGeneral?.Invoke(this, interactions);
        }

        public async Task CreateGame(
            Dictionary<string, int> deckList1,
            Dictionary<string, int> deckList2,
            string player1Name,
            string player2Name,
            string logFilePath
        )
        {
            _actionSystem.SetupLogFile(logFilePath);
            await _actionSystem.Perform(
                new CreateGameGA(deckList1, deckList2, player1Name, player2Name)
            );
        }

        public async Task RecreateGameFromLog(string logFilePath)
        {
            await _actionSystem.RecreateGameStateFromLog(logFilePath);
        }

        public void StartGame()
        {
            _game.GameState.OnAdvanced(_game);
        }

        public async Task SelectActivePokemon(IPokemonCardLogic basicPokemon)
        {
            await _actionSystem.Perform(new SetActivePokemonGA(basicPokemon));
            await _game.AdvanceGameState();
        }

        public async Task PlayCard(ICardLogic card)
        {
            await _actionSystem.Perform(new PlayCardGA(card));
            await _game.AdvanceGameState();
        }

        public async Task PlayCardWithTargets(ICardLogic card, List<ICardLogic> targets)
        {
            await _actionSystem.Perform(new PlayCardGA(card, targets));
            await _game.AdvanceGameState();
        }

        public async Task SelectMulligans(int numberOfExtraCards, IPlayerLogic player)
        {
            await _actionSystem.Perform(new DrawCardGA(numberOfExtraCards, player));
            await _game.AdvanceGameState();
        }

        public async Task PerformAttack(IAttackLogic attack, IPokemonCardLogic attacker)
        {
            await _actionSystem.Perform(new AttackGA(attack, attacker));
            await _game.AdvanceGameState();
        }

        internal async Task EndTurn()
        {
            await _actionSystem.Perform(new EndTurnGA());
            await _game.AdvanceGameState();
        }

        internal async Task Confirm()
        {
            await _game.AdvanceGameState();
        }

        internal async Task Retreat(
            IPokemonCardLogic pokemon,
            List<IEnergyCardLogic> energyCardsToDiscard
        )
        {
            await _actionSystem.Perform(new RetreatGA(pokemon, energyCardsToDiscard));
            await _game.AdvanceGameState();
        }

        internal async Task PerformAbility(IPokemonCardLogic pokemon)
        {
            await _actionSystem.Perform(new PerformAbilityGA(pokemon));
            await _game.AdvanceGameState();
        }

        public Task<CreateGameGA> Perform(CreateGameGA action)
        {
            _game = new GameBuilder()
                .WithPlayer1(action.Player1Name)
                .WithPlayer2(action.Player2Name)
                .WithPlayer1Decklist(action.DeckList1)
                .WithPlayer2Decklist(action.DeckList2)
                .Build();
            _game.AwaitInteractionEvent += NotifyPlayers;
            _game.AwaitGeneralInteractionEvent += OnExpectGeneralInteraction;
            return Task.FromResult(action);
        }

        internal async Task SetPrizeCards()
        {
            await _actionSystem.Perform(new SetPrizeCardsGA());
            await _game.AdvanceGameState();
        }
    }
}
