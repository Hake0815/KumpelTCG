using System;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using gamecore.instruction.filter;

namespace gamecore.instruction
{
    class SelectCardsFromDiscardPileInstruction : SelectCardsInstruction
    {
        public SelectCardsFromDiscardPileInstruction(IntRange countRange, FilterNode filter)
            : base(countRange, filter) { }

        public override void Perform(ICardLogic card)
        {
            ActionSystem.INSTANCE.AddReaction(
                new ConfirmSelectCardsGA(
                    card.Owner,
                    CountRange.Contains,
                    card.Owner.DiscardPile,
                    c => Filter.Matches(c, card),
                    SelectCardsGA.SelectedCardsOrigin.DiscardPile
                )
            );
        }
    }
}
