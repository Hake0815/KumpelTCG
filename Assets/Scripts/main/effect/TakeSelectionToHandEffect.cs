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
        public int Amount { get; }

        public TakeSelectionToHandEffect(int amount)
        {
            Amount = amount;
        }

        public void Perform(ICardLogic card)
        {
            new EffectSubscriber<RevealCardsFromDeckGA>(
                action => Reaction(action, card),
                ReactionTiming.POST
            );
        }

        private RevealCardsFromDeckGA Reaction(RevealCardsFromDeckGA action, ICardLogic card)
        {
            ActionSystem.INSTANCE.AddReaction(
                new TakeSelectionToHandGA(action.RevealedCards, card.Owner, Amount)
            );
            return action;
        }
    }
}
