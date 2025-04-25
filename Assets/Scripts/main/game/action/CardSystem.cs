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

        private readonly ActionSystem _actionSystem = ActionSystem.INSTANCE;

        public void Enable()
        {
            _actionSystem.AttachPerformer<DrawCardGA>(INSTANCE);
            _actionSystem.AttachPerformer<DiscardCardsFromHandGA>(INSTANCE);
            _actionSystem.AttachPerformer<AttachEnergyFromHandGA>(INSTANCE);
            _actionSystem.AttachPerformer<AttachEnergyFromHandForTurnGA>(INSTANCE);
            _actionSystem.SubscribeToGameAction<EndTurnGA>(INSTANCE, ReactionTiming.POST);
        }

        public void Disable()
        {
            _actionSystem.DetachPerformer<DrawCardGA>();
            _actionSystem.DetachPerformer<DiscardCardsFromHandGA>();
            _actionSystem.DetachPerformer<AttachEnergyFromHandGA>();
            _actionSystem.DetachPerformer<AttachEnergyFromHandForTurnGA>();
            _actionSystem.UnsubscribeFromGameAction<EndTurnGA>(INSTANCE, ReactionTiming.POST);
        }

        public EndTurnGA React(EndTurnGA endTurnGA)
        {
            var drawCardGA = new DrawCardGA(1, endTurnGA.NextPlayer);
            _actionSystem.AddReaction(drawCardGA);
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
