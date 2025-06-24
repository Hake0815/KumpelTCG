using System;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using UnityEngine;

namespace gamecore.effect
{
    class PutRemainingCardsUnderDeckEffect : IEffect
    {
        public void Perform(ICardLogic card)
        {
            new EffectSubscriber<TakeSelectionToHandGA>(
                action => Reaction(action, card),
                ReactionTiming.POST
            );
        }

        private static TakeSelectionToHandGA Reaction(TakeSelectionToHandGA action, ICardLogic card)
        {
            ActionSystem.INSTANCE.AddReaction(
                new PutRemainingCardsUnderDeckGA(card.Owner, action.RemainingCards)
            );
            return action;
        }
    }
}
