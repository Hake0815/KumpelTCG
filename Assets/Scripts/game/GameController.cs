using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.game
{
    public interface IGameController
    {
        IGame Game { get; }
        event EventHandler<List<GameInteraction>> NotifyPlayer1;
        event EventHandler<List<GameInteraction>> NotifyPlayer2;
        void SetUpGame();
    }

    internal class GameController : IGameController
    {
        private Game _game;

        public event EventHandler<List<GameInteraction>> NotifyPlayer1;
        public event EventHandler<List<GameInteraction>> NotifyPlayer2;

        public IGame Game
        {
            get { return _game; }
        }

        public GameController(Game game)
        {
            _game = game;
            _game.AwaitInteractionEvent += NotifyPlayers;
        }

        public void SetUpGame()
        {
            _game.PerformSetup();
            NotifyPlayers();
        }

        public void SelectActivePokemon(ICardLogic basicPokemon)
        {
            if (_game.SetActivePokemon(basicPokemon))
                NotifyPlayers();
        }

        public void PlayCard(ICardLogic card)
        {
            _game.PlayCard(card);
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
                NotifyPlayer1?.Invoke(this, interactions);
        }

        protected virtual void OnNotifyPlayer2()
        {
            var interactions = _game.GameState.GetGameInteractions(this, _game.Player2);
            if (interactions.Count > 0)
                NotifyPlayer2?.Invoke(this, interactions);
        }

        internal void EndTurn()
        {
            if (ActionSystem.INSTANCE.IsPerforming)
                return;
            var endTurn = new EndTurnGA();
            ActionSystem.INSTANCE.Perform(endTurn);
        }
    }
}
