using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gamecore.card;
using gamecore.game;
using UnityEngine;
using UnityEngine.Splines;

namespace gameview
{
    public class HandView : MonoBehaviour, ISplineCardHolder
    {
        public SplineContainer SplineContainer { get; private set; }
        private ICardList hand;

        private void Awake()
        {
            SplineContainer = GetComponent<SplineContainer>();
        }

        public void Register(IPlayer player)
        {
            hand = player.Hand;
            OnEnable();
        }

        private void OnEnable()
        {
            if (hand != null)
            {
                hand.CardCountChanged += HandleCardCountChanged;
            }
        }

        private async void HandleCardCountChanged(List<ICard> cards)
        {
            await ((ISplineCardHolder)this).UpdateCardPosition(cards, transform.rotation);
        }

        private void OnDisable()
        {
            if (hand != null)
            {
                hand.CardCountChanged -= HandleCardCountChanged;
            }
        }

        public async Task HandleCardCountChanged()
        {
            await ((ISplineCardHolder)this).UpdateCardPosition(hand.Cards, transform.rotation);
        }
    }
}
