using System;
using System.Collections.Generic;
using gamecore.card;

namespace gameview
{
    public class CardViewComparer : IComparer<CardView>
    {
        private readonly Predicate<ICard> _cardCondition;

        private CardViewComparer(Predicate<ICard> cardCondition)
        {
            _cardCondition = cardCondition;
        }

        public static CardViewComparer Create()
        {
            return new CardViewComparer(null);
        }

        public static CardViewComparer Create(Predicate<ICard> cardCondition)
        {
            return new CardViewComparer(cardCondition);
        }

        public int Compare(CardView x, CardView y)
        {
            return -x.Card.CompareTo(y.Card, _cardCondition);
        }
    }
}
