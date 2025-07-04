using System;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using UnityEngine;

namespace gamecore.effect
{
    class TakeSelectionToHandEffect : IEffect
    {
        public void Perform(ICardLogic card)
        {
            new EffectSubscriber<SelectCardsGA>(
                action => Reaction(action, card),
                ReactionTiming.POST
            );
        }

        private static bool Reaction(SelectCardsGA action, ICardLogic card)
        {
            if (action.WasReactedTo)
                return false;
            ActionSystem.INSTANCE.AddReaction(
                new TakeSelectionToHandGA(action.SelectedCards, card.Owner, action.RemainingCards)
            );
            action.WasReactedTo = true;
            return true;
        }
    }
}
