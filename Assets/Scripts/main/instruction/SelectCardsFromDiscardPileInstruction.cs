using System;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.instruction
{
    class SelectCardsFromDiscardPileInstruction : IInstruction
    {
        public SelectCardsFromDiscardPileInstruction(
            Predicate<int> numberOfCardsCondition,
            Predicate<ICardLogic> cardCondition
        )
        {
            NumberOfCardsCondition = numberOfCardsCondition;
            CardCondition = cardCondition;
        }

        public Predicate<int> NumberOfCardsCondition { get; }
        public Predicate<ICardLogic> CardCondition { get; }

        public void Perform(ICardLogic card)
        {
            ActionSystem.INSTANCE.AddReaction(
                new ConfirmSelectCardsGA(
                    card.Owner,
                    NumberOfCardsCondition,
                    card.Owner.DiscardPile,
                    CardCondition,
                    SelectCardsGA.SelectedCardsOrigin.DiscardPile
                )
            );
        }
    }
}
