using System;
using gamecore.card;

namespace gamecore.instruction
{
    class HasAtLeastCardsOfTypeInDiscardPile : IUseCondition
    {
        private readonly int _count;
        private readonly Predicate<ICardLogic> _cardCondition;

        public HasAtLeastCardsOfTypeInDiscardPile(int count, Predicate<ICardLogic> cardCondition)
        {
            _count = count;
            _cardCondition = cardCondition;
        }

        public bool IsMet(ICardLogic card)
        {
            var counter = 0;
            foreach (var discardPileCard in card.Owner.DiscardPile.Cards)
            {
                if (_cardCondition(discardPileCard))
                    counter++;
                if (counter >= _count)
                    return true;
            }
            return false;
        }
    }
}
