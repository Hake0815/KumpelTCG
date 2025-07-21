using System;
using gamecore.card;
using gamecore.instruction.filter;

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

        public ConditionJson ToSerializable()
        {
            return new ConditionJson(
                conditionType: "has_cards",
                new()
                {
                    { "count", _count },
                    { "in", "discard_pile" },
                    { "filter", _filter.ToSerializable() },
                }
            );
        }
    }
}
