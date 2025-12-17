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

        public ConditionJson ToSerializable()
        {
            return new ConditionJson(
                conditionType: ConditionType.HasCards,
                new()
                {
                    new InstructionDataJson(
                        InstructionDataType.CardAmountData,
                        new CardAmountInstructionDataJson(
                            new IntRange(Count, 60),
                            CardPosition.Hand
                        )
                    ),
                }
            );
        }
    }
}
