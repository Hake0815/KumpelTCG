using System;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using UnityEngine;

namespace gamecore.game.action
{
    internal class CardSystem
        : IActionPerformer<DrawCardGA>,
            IActionPerformer<DiscardCardsFromHandGA>,
            IActionPerformer<AttachEnergyFromHandGA>,
            IActionPerformer<AttachEnergyFromHandForTurnGA>,
            IActionPerformer<DiscardAttachedEnergyCardsGA>,
            IActionPerformer<BenchPokemonGA>,
            IActionPerformer<MovePokemonToBenchGA>,
            IActionPerformer<EvolveGA>,
            IActionPerformer<ResetPokemonInPlayStateGA>,
            IActionSubscriber<StartTurnGA>
    {
        private static readonly Lazy<CardSystem> lazy = new(() => new CardSystem());
        public static CardSystem INSTANCE => lazy.Value;

        private CardSystem() { }

        private readonly ActionSystem _actionSystem = ActionSystem.INSTANCE;

        public void Enable()
        {
            _actionSystem.AttachPerformer<DrawCardGA>(INSTANCE);
            _actionSystem.AttachPerformer<DiscardCardsFromHandGA>(INSTANCE);
            _actionSystem.AttachPerformer<AttachEnergyFromHandGA>(INSTANCE);
            _actionSystem.AttachPerformer<AttachEnergyFromHandForTurnGA>(INSTANCE);
            _actionSystem.AttachPerformer<DiscardAttachedEnergyCardsGA>(INSTANCE);
            _actionSystem.AttachPerformer<BenchPokemonGA>(INSTANCE);
            _actionSystem.AttachPerformer<MovePokemonToBenchGA>(INSTANCE);
            _actionSystem.AttachPerformer<EvolveGA>(INSTANCE);
            _actionSystem.AttachPerformer<ResetPokemonInPlayStateGA>(INSTANCE);
            _actionSystem.SubscribeToGameAction<StartTurnGA>(INSTANCE, ReactionTiming.POST);
        }

        public void Disable()
        {
            _actionSystem.DetachPerformer<DrawCardGA>();
            _actionSystem.DetachPerformer<DiscardCardsFromHandGA>();
            _actionSystem.DetachPerformer<AttachEnergyFromHandGA>();
            _actionSystem.DetachPerformer<AttachEnergyFromHandForTurnGA>();
            _actionSystem.DetachPerformer<DiscardAttachedEnergyCardsGA>();
            _actionSystem.DetachPerformer<BenchPokemonGA>();
            _actionSystem.DetachPerformer<EvolveGA>();
            _actionSystem.DetachPerformer<ResetPokemonInPlayStateGA>();
            _actionSystem.UnsubscribeFromGameAction<StartTurnGA>(INSTANCE, ReactionTiming.POST);
        }

        public StartTurnGA React(StartTurnGA startTurnGA)
        {
            var drawCardGA = new DrawCardGA(1, startTurnGA.NextPlayer);
            _actionSystem.AddReaction(drawCardGA);
            return startTurnGA;
        }

        public Task<DrawCardGA> Perform(DrawCardGA drawCardGA)
        {
            ((IPlayerLogic)drawCardGA.Player).Draw(drawCardGA.Amount);
            return Task.FromResult(drawCardGA);
        }

        public Task<DiscardCardsFromHandGA> Perform(DiscardCardsFromHandGA action)
        {
            foreach (var card in action.Cards)
            {
                card.Discard();
                card.Owner.Hand.RemoveCards(new() { card });
            }
            return Task.FromResult(action);
        }

        public Task<AttachEnergyFromHandGA> Perform(AttachEnergyFromHandGA action)
        {
            AttachEnergyFromHand(action);
            return Task.FromResult(action);
        }

        public Task<AttachEnergyFromHandForTurnGA> Perform(AttachEnergyFromHandForTurnGA action)
        {
            AttachEnergyFromHand(action);
            action.EnergyCard.Owner.PerformedOncePerTurnActions.Add(
                EnergyCard.ATTACHED_ENERGY_FOR_TURN
            );
            return Task.FromResult(action);
        }

        private static void AttachEnergyFromHand(AttachEnergyFromHandGA action)
        {
            var energyCard = action.EnergyCard;
            energyCard.Owner.Hand.RemoveCard(energyCard);
            action.TargetPokemon.AttachEnergyCards(new() { energyCard });
        }

        public Task<DiscardAttachedEnergyCardsGA> Perform(DiscardAttachedEnergyCardsGA action)
        {
            action.Pokemon.DiscardEnergy(action.EnergyCards);
            return Task.FromResult(action);
        }

        public Task<BenchPokemonGA> Perform(BenchPokemonGA action)
        {
            var pokemon = action.Card;
            pokemon.Owner.Bench.AddCards(new() { pokemon });
            pokemon.Owner.Hand.RemoveCard(pokemon);
            return Task.FromResult(action);
        }

        public Task<MovePokemonToBenchGA> Perform(MovePokemonToBenchGA action)
        {
            var pokemon = action.Pokemon;
            pokemon.Owner.Bench.AddCards(new() { pokemon });
            return Task.FromResult(action);
        }

        public Task<EvolveGA> Perform(EvolveGA action)
        {
            action.NewPokemon.Owner.Hand.RemoveCard(action.NewPokemon);
            action.NewPokemon.PreEvolutions.Add(action.TargetPokemon);

            if (action.TargetPokemon == action.TargetPokemon.Owner.ActivePokemon)
                action.TargetPokemon.Owner.ActivePokemon = action.NewPokemon;
            else
            {
                action.TargetPokemon.Owner.Bench.RemoveCard(action.TargetPokemon);
                action.NewPokemon.Owner.Bench.AddCards(new() { action.NewPokemon });
            }

            action.NewPokemon.AttachEnergyCards(action.TargetPokemon.AttachedEnergyCards);
            action.TargetPokemon.AttachedEnergyCards.Clear();

            foreach (var preEvolution in action.TargetPokemon.PreEvolutions)
                action.NewPokemon.PreEvolutions.Add(preEvolution);

            action.TargetPokemon.PreEvolutions.Clear();

            action.TargetPokemon.WasEvolved();
            return Task.FromResult(action);
        }

        public Task<ResetPokemonInPlayStateGA> Perform(ResetPokemonInPlayStateGA action)
        {
            action.PokemonToReset.PutIntoPlayThisTurn = false;
            ActionSystem.INSTANCE.UnsubscribeFromGameAction<StartTurnGA>(
                action.PokemonToReset,
                ReactionTiming.PRE
            );
            return Task.FromResult(action);
        }
    }
}
