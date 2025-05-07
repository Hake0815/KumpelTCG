using System;
using System.Collections.Generic;
using gamecore.card;
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
#pragma warning disable 4014
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

        public void PlayCardWithTargets(ICardLogic card, List<ICardLogic> targets)
        {
            _game.PlayCardWithTargets(card, targets);
        }

        public void SelectMulligans(int numberOfExtraCards, IPlayerLogic player)
        {
            _game.DrawMulliganCards(numberOfExtraCards, player);
        }

        public void PerformAttack(IAttackLogic attack, IPokemonCardLogic attacker)
        {
            _game.PerformAttack(attack, attacker);
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

        internal void Retreat(
            IPokemonCardLogic pokemon,
            List<IEnergyCardLogic> energyCardsToDiscard
        )
        {
            _game.Retreat(pokemon, energyCardsToDiscard);
        }
    }
#pragma warning restore 4014
}
