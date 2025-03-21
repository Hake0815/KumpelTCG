using System;
using gamecore.actionsystem;
using UnityEngine;

namespace gamecore.game.action
{
    public class CardSystem
        : IActionPerformer<DrawCardGA>,
            IActionPerformer<DiscardCardsFromHandGA>,
            IActionSubscriber<EndTurnGA>
    {
        private static readonly Lazy<CardSystem> lazy = new(() => new CardSystem());
        public static CardSystem INSTANCE => lazy.Value;

        private CardSystem() { }

        public void Enable()
        {
            ActionSystem.INSTANCE.AttachPerformer<DrawCardGA>(INSTANCE);
            ActionSystem.INSTANCE.AttachPerformer<DiscardCardsFromHandGA>(INSTANCE);
            ActionSystem.INSTANCE.SubscribeToGameAction<EndTurnGA>(INSTANCE, ReactionTiming.POST);
        }

        public void Disable()
        {
            ActionSystem.INSTANCE.DetachPerformer<DrawCardGA>();
            ActionSystem.INSTANCE.DetachPerformer<DiscardCardsFromHandGA>();
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
            for (int i = 0; i < drawCardGA.Amount; i++)
            {
                drawCardGA.Player.Draw();
            }
            return drawCardGA;
        }

        public DiscardCardsFromHandGA Perform(DiscardCardsFromHandGA action)
        {
            Debug.Log("Discarding cards from hand: " + action.Cards.Count);
            foreach (var card in action.Cards)
            {
                Debug.Log("Discarding card: " + card.Name);
                card.Discard();
                card.Owner.RemoveCardFromHand(card);
            }
            return action;
        }
    }
}
