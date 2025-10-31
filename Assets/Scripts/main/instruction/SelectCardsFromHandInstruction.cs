using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using gamecore.game.interaction;
using gamecore.instruction.filter;

namespace gamecore.instruction
{
    class SelectCardsFromHandInstruction : SelectCardsInstruction
    {
        public SelectCardsFromHandInstruction(
            IntRange countRange,
            FilterNode filter,
            string selectionId,
            ActionOnSelection targetAction,
            ActionOnSelection remainderAction
        )
            : base(countRange, filter, selectionId, targetAction, remainderAction) { }

        public override void Perform(ICardLogic card, ActionSystem actionSystem)
        {
            actionSystem.AddReaction(
                new QuickSelectCardsGA(
                    card.Owner,
                    new ConditionalTargetQuery(
                        new NumberRange(CountRange.Min, CountRange.Max),
                        SelectionQualifier.NumberOfCards
                    ),
                    card.Owner.Hand.Cards,
                    c => Filter.Matches(c, card),
                    SelectCardsGA.SelectedCardsOrigin.Hand,
                    SelectionId,
                    TargetAction,
                    RemainderAction
                )
            );
        }

        public override InstructionJson ToSerializable()
        {
            return new InstructionJson(
                instructionType: InstructionType.SelectCards,
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
