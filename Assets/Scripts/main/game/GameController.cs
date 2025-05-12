using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        Task SetUpGame();
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

        public async Task SetUpGame()
        {
            await _game.PerformSetup();
        }

        public async Task SelectActivePokemon(ICardLogic basicPokemon)
        {
            await _game.SetActivePokemon(basicPokemon);
        }

        public async Task PlayCard(ICardLogic card)
        {
            await _game.PlayCard(card);
        }

        public async Task PlayCardWithTargets(ICardLogic card, List<ICardLogic> targets)
        {
            await _game.PlayCardWithTargets(card, targets);
        }

        public async Task SelectMulligans(int numberOfExtraCards, IPlayerLogic player)
        {
            await _game.DrawMulliganCards(numberOfExtraCards, player);
        }

        public async Task PerformAttack(IAttackLogic attack, IPokemonCardLogic attacker)
        {
            await _game.PerformAttack(attack, attacker);
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

        internal async Task EndTurn()
        {
            await _game.EndTurn();
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
            await _game.Retreat(pokemon, energyCardsToDiscard);
        }
    }
}
