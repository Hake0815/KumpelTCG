using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using gamecore.instruction.filter;

namespace gamecore.instruction
{
    class SelectCardsFromHandInstruction : SelectCardsInstruction
    {
        public SelectCardsFromHandInstruction(IntRange countRange, FilterNode filter)
            : base(countRange, filter) { }

        public override void Perform(ICardLogic card)
        {
            ActionSystem.INSTANCE.AddReaction(
                new QuickSelectCardsGA(
                    card.Owner,
                    CountRange.Contains,
                    card.Owner.Hand,
                    c => Filter.Matches(c, card),
                    SelectCardsGA.SelectedCardsOrigin.Hand
                )
            );
        }

        public override InstructionJson ToSerializable()
        {
            return new InstructionJson(
                instructionType: "select_cards",
                data: new Dictionary<string, object>
                {
                    { "from", "hand" },
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
