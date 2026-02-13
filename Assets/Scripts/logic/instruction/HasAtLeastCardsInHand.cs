using gamecore.card;
using gamecore.instruction.filter;
using gamecore.serialization;

namespace gamecore.instruction
{
    class HasAtLeastCardsInHand : IUseCondition
    {
        public int Count { get; }

        public HasAtLeastCardsInHand(int count)
        {
            Count = count;
        }

        public bool IsMet(ICardLogic card)
        {
            return card.Owner.Hand.CardCount >= Count;
        }

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
                            Amount = new ProtoBufIntRange { Min = Count, Max = 60 },
                            FromPosition = ProtoBufCardPosition.CardPositionHand,
                        },
                    },
                },
            };
        }
    }
}
