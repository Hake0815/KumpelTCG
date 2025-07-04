using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.effect
{
    class SelectFromRevealedCardsEffect : IEffect
    {
        public int Amount { get; }

        public SelectFromRevealedCardsEffect(int amount)
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

        private bool Reaction(RevealCardsFromDeckGA action, ICardLogic card)
        {
            ActionSystem.INSTANCE.AddReaction(
                new SelectExactCardsGA(
                    card.Owner,
                    Amount,
                    new CardListLogic(action.RevealedCards),
                    SelectCardsGA.SelectedCardsOrigin.Other
                )
            );
            return true;
        }
    }
}
