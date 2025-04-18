using System;
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
            IActionSubscriber<EndTurnGA>
    {
        private static readonly Lazy<CardSystem> lazy = new(() => new CardSystem());
        public static CardSystem INSTANCE => lazy.Value;

        private CardSystem() { }

        public void Enable()
        {
            ActionSystem.INSTANCE.AttachPerformer<DrawCardGA>(INSTANCE);
            ActionSystem.INSTANCE.AttachPerformer<DiscardCardsFromHandGA>(INSTANCE);
            ActionSystem.INSTANCE.AttachPerformer<AttachEnergyFromHandGA>(INSTANCE);
            ActionSystem.INSTANCE.AttachPerformer<AttachEnergyFromHandForTurnGA>(INSTANCE);
            ActionSystem.INSTANCE.SubscribeToGameAction<EndTurnGA>(INSTANCE, ReactionTiming.POST);
        }

        public void Disable()
        {
            ActionSystem.INSTANCE.DetachPerformer<DrawCardGA>();
            ActionSystem.INSTANCE.DetachPerformer<DiscardCardsFromHandGA>();
            ActionSystem.INSTANCE.DetachPerformer<AttachEnergyFromHandGA>();
            ActionSystem.INSTANCE.DetachPerformer<AttachEnergyFromHandForTurnGA>();
            ActionSystem.INSTANCE.UnsubscribeFromGameAction<EndTurnGA>(
                INSTANCE,
                ReactionTiming.POST
            );
        }

        public EndTurnGA React(EndTurnGA endTurnGA)
        {
            var drawCardGA = new DrawCardGA(1, endTurnGA.NextPlayer);
            ActionSystem.INSTANCE.AddReaction(drawCardGA);
            return endTurnGA;
        }

        public DrawCardGA Perform(DrawCardGA drawCardGA)
        {
            ((IPlayerLogic)drawCardGA.Player).Draw(drawCardGA.Amount);
            return drawCardGA;
        }

        public DiscardCardsFromHandGA Perform(DiscardCardsFromHandGA action)
        {
            foreach (var card in action.Cards)
            {
                card.Discard();
                card.Owner.Hand.RemoveCards(new() { card });
            }
            return action;
        }

        public AttachEnergyFromHandGA Perform(AttachEnergyFromHandGA action)
        {
            AttachEnergyFromHand(action);
            return action;
        }

        public AttachEnergyFromHandForTurnGA Perform(AttachEnergyFromHandForTurnGA action)
        {
            AttachEnergyFromHand(action);
            action.EnergyCard.Owner.PerformedOncePerTurnActions.Add(
                EnergyCard.ATTACHED_ENERGY_FOR_TURN
            );
            return action;
        }

        private static void AttachEnergyFromHand(AttachEnergyFromHandGA action)
        {
            var energyCard = action.EnergyCard;
            energyCard.Owner.Hand.RemoveCard(energyCard);
            action.TargetPokemon.AttachEnergy(energyCard);
        }
    }
}
