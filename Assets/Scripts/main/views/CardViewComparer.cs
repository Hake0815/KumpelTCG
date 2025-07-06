using System;
using System.Collections.Generic;
using gamecore.card;
using UnityEngine;

namespace gameview
{
    public class CardViewComparer : IComparer<CardView>
    {
        private readonly Predicate<ICard> _cardCondition;
        private readonly Predicate<CardView> _cardViewCondition;

        private CardViewComparer(
            Predicate<ICard> cardCondition,
            Predicate<CardView> cardViewCondition
        )
        {
            _cardCondition = cardCondition;
            _cardViewCondition = cardViewCondition;
        }

        public static CardViewComparer Create()
        {
            return new CardViewComparer(null, null);
        }

        public static CardViewComparer Create(Predicate<ICard> cardCondition)
        {
            return new CardViewComparer(cardCondition, null);
        }

        public static CardViewComparer Create(Predicate<CardView> cardViewCondition)
        {
            return new CardViewComparer(null, cardViewCondition);
        }

        public int Compare(CardView x, CardView y)
        {
            if (_cardViewCondition is not null)
            {
                if (_cardViewCondition(x) && !_cardViewCondition(y))
                {
                    Debug.Log(
                        $"CardViewComparer compared {x.Card.Name} and {y.Card.Name}, returning -1 because {x.Card.Name} matches condition"
                    );
                    return -1;
                }
                if (!_cardViewCondition(x) && _cardViewCondition(y))
                {
                    Debug.Log(
                        $"CardViewComparer compared {x.Card.Name} and {y.Card.Name}, returning 1 because {y.Card.Name} matches condition"
                    );
                    return 1;
                }

                Debug.Log(
                    $"CardViewComparer compared {x.Card.Name} and {y.Card.Name}, both don't match condition"
                );
            }
            return -x.Card.CompareTo(y.Card, _cardCondition);
        }
    }
}
