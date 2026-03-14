using System;
using gamecore.card;
using gamecore.instruction.filter;
using gamecore.serialization;

namespace gamecore.instruction
{
    class HasAtLeastCardsOfTypeInDiscardPile : IUseCondition
    {
        private readonly int _count;
        private readonly FilterNode _filter;

        public HasAtLeastCardsOfTypeInDiscardPile(int count, FilterNode cardCondition)
        {
            _count = count;
            _filter = cardCondition;
        }

        public bool IsMet(ICardLogic card)
        {
            var counter = 0;
            foreach (var discardPileCard in card.Owner.DiscardPile.Cards)
            {
                if (_filter.Matches(discardPileCard, card))
                    counter++;
                if (counter >= _count)
                    return true;
            }
            return false;
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
                            Amount = new ProtoBufIntRange { Min = _count, Max = 60 },
                            FromPosition = ProtoBufCardPosition.CardPositionDiscardPile,
                        },
                    },
                    new ProtoBufInstructionData
                    {
                        InstructionDataType =
                            ProtoBufInstructionDataType.InstructionDataTypeFilterData,
                        FilterData = new ProtoBufFilterInstructionData
                        {
                            Filter = _filter.ToSerializable(),
                        },
                    },
                },
            };
        }
    }
}
