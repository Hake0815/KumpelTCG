using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using gamecore.game.interaction;
using gamecore.instruction.filter;
using gamecore.serialization;

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

        public override ProtoBufInstruction ToSerializable()
        {
            return new ProtoBufInstruction
            {
                InstructionType = ProtoBufInstructionType.InstructionTypeSelectCards,
                Data =
                {
                    new ProtoBufInstructionData
                    {
                        InstructionDataType =
                            ProtoBufInstructionDataType.InstructionDataTypeCardAmountData,
                        CardAmountData = new ProtoBufCardAmountInstructionData
                        {
                            Amount = new ProtoBufIntRange
                            {
                                Min = CountRange.Min,
                                Max = CountRange.Max,
                            },
                            FromPosition = ProtoBufCardPosition.CardPositionHand,
                        },
                    },
                    new ProtoBufInstructionData
                    {
                        InstructionDataType =
                            ProtoBufInstructionDataType.InstructionDataTypeFilterData,
                        FilterData = new ProtoBufFilterInstructionData
                        {
                            Filter = Filter.ToSerializable(),
                        },
                    },
                },
            };
        }
    }
}
