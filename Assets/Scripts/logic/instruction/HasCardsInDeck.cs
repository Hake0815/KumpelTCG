using gamecore.card;
using gamecore.instruction.filter;
using gamecore.serialization;

namespace gamecore.instruction
{
    class HasCardsInDeck : IUseCondition
    {
        public bool IsMet(ICardLogic card) => card.Owner.Deck.CardCount > 0;

        public ProtoBufCondition ToSerializable()
        {
            return new ProtoBufCondition
            {
                ConditionType = ProtoBufConditionType.ConditionTypeHasCards,
                Data =
                {
                    new ProtoBufInstructionData
                    {
                        InstructionDataType =
                            ProtoBufInstructionDataType.InstructionDataTypeCardAmountData,
                        CardAmountData = new ProtoBufCardAmountInstructionData
                        {
                            Amount = new ProtoBufIntRange { Min = 1, Max = 60 },
                            FromPosition = ProtoBufCardPosition.CardPositionDeck,
                        },
                    },
                },
            };
        }
    }
}
