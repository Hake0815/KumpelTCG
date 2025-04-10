using System;
using System.Collections.Generic;
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

        void SetUpGame();
    }

    internal class GameController : IGameController
    {
        private readonly Game _game;

        public event EventHandler<List<GameInteraction>> NotifyPlayer1;
        public event EventHandler<List<GameInteraction>> NotifyPlayer2;
        public event EventHandler<List<GameInteraction>> NotifyGeneral;

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

        public void SetUpGame()
        {
            _game.PerformSetup();
        }

        public void SelectActivePokemon(ICardLogic basicPokemon)
        {
            _game.SetActivePokemon(basicPokemon);
        }

        public void PlayCard(ICardLogic card)
        {
            _game.PlayCard(card);
        }

        public void SelectMulligans(int numberOfExtraCards, IPlayerLogic player)
        {
            _game.DrawMulliganCards(numberOfExtraCards, player);
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

        internal void EndTurn()
        {
            _game.EndTurn();
        }

        internal void Confirm()
        {
            _game.AdvanceGameState();
        }
    }
}
