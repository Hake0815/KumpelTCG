using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using gamecore.game.interaction;
using gamecore.instruction.filter;
using gamecore.serialization;

namespace gamecore.instruction
{
    class SelectCardsFromDeckInstruction : SelectCardsInstruction
    {
        public SelectCardsFromDeckInstruction(
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
                new ConfirmSelectCardsGA(
                    card.Owner,
                    new ConditionalTargetQuery(
                        new NumberRange(CountRange.Min, CountRange.Max),
                        SelectionQualifier.NumberOfCards
                    ),
                    card.Owner.Deck.Cards,
                    c => Filter.Matches(c, card),
                    SelectCardsGA.SelectedCardsOrigin.Deck,
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
                            FromPosition = ProtoBufCardPosition.CardPositionDeck,
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
