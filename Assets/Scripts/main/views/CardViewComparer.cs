using System;
using System.Collections.Generic;
using gamecore.card;
using UnityEngine;

namespace gameview
{
    public class CardViewComparer : IComparer<CardView>
    {
        private readonly Predicate<ICard> _cardCondition;

        private CardViewComparer(
            Predicate<ICard> cardCondition,
            Predicate<CardView> cardViewCondition
        )
        {
            _cardCondition = cardCondition;
        }

        public static CardViewComparer Create()
        {
            return new CardViewComparer(null, null);
        }

        public static CardViewComparer Create(Predicate<ICard> cardCondition)
        {
            return new CardViewComparer(cardCondition, null);
        }

        public int Compare(CardView x, CardView y)
        {
            return -x.Card.CompareTo(y.Card, _cardCondition);
        }
    }
}
