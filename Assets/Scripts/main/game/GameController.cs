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

        Task SetUpGame();
    }

    class GameController : IGameController
    {
        private readonly Game _game;

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

        public async Task SetUpGame()
        {
            await _actionSystem.Perform(new SetupGA());
            await _game.AdvanceGameState();
        }

        public async Task SelectActivePokemon(ICardLogic basicPokemon)
        {
            await _actionSystem.Perform(new PlayCardGA(basicPokemon));
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
    }
}
