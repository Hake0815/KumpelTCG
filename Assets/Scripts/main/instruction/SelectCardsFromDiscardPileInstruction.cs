using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using gamecore.instruction.filter;

namespace gamecore.instruction
{
    class SelectCardsFromDiscardPileInstruction : SelectCardsInstruction
    {
        public SelectCardsFromDiscardPileInstruction(
            IntRange countRange,
            FilterNode filter,
            string selectionId
        )
            : base(countRange, filter, selectionId) { }

        public override void Perform(ICardLogic card, ActionSystem actionSystem)
        {
            actionSystem.AddReaction(
                new ConfirmSelectCardsGA(
                    card.Owner,
                    CountRange.Contains,
                    card.Owner.DiscardPile,
                    c => Filter.Matches(c, card),
                    SelectCardsGA.SelectedCardsOrigin.DiscardPile,
                    SelectionId
                )
            );
        }

        public override InstructionJson ToSerializable()
        {
            return new InstructionJson(
                instructionType: "select_cards",
                data: new Dictionary<string, object>
                {
                    { "from", "discard_pile" },
                    {
                        "count",
                        new Dictionary<string, object>
                        {
                            { "min", CountRange.Min },
                            { "max", CountRange.Max },
                        }
                    },
                    { "filter", Filter.ToSerializable() },
                }
            );
        }
    }
}
