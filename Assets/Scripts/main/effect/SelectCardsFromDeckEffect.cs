using System;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.effect
{
    class SelectCardsFromDeckEffect : IEffect
    {
        public int Amount { get; }
        public Predicate<ICardLogic> CardCondition { get; }

        public SelectCardsFromDeckEffect(int amount, Predicate<ICardLogic> cardCondition)
        {
            Amount = amount;
            CardCondition = cardCondition;
        }

        public void Perform(ICardLogic card)
        {
            // ActionSystem.INSTANCE.AddReaction(
            //     new SelectCardsGA(
            //         card.Owner,
            //         Amount,
            //         card.Owner.Hand,
            //         CardCondition,
            //         SelectCardsGA.SelectedCardsOrigin.Deck
            //     )
            // );
        }
    }
}
