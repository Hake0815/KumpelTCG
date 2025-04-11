using System.Collections.Generic;
using gamecore.card;
using gamecore.game;
using UnityEngine;
using UnityEngine.Splines;

namespace gameview
{
    public class HandView : MonoBehaviour, ISplineCardHolder
    {
        private Transform deckPosition;
        public SplineContainer SplineContainer { get; private set; }
        private ICardList hand;

        private void Awake()
        {
            SplineContainer = GetComponent<SplineContainer>();
        }

        public void SetUp(DeckView deck)
        {
            deckPosition = deck.transform;
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
                hand.CardsAdded += HandleCardsAdded;
                hand.CardCountChanged += HandleCardsRemoved;
            }
        }

        private void OnDisable()
        {
            if (hand != null)
            {
                hand.CardsAdded -= HandleCardsAdded;
                hand.CardCountChanged -= HandleCardsRemoved;
            }
        }

        private void HandleCardsRemoved()
        {
            ((ISplineCardHolder)this).UpdateCardPosition(
                CardViewRegistry.INSTANCE.GetAll(hand.Cards),
                transform.rotation
            );
        }

        public void CreateHandCards()
        {
            HandleCardsAdded(this, hand.Cards);
        }

        private void HandleCardsAdded(object player, List<ICard> drawnCards)
        {
            foreach (var card in drawnCards)
            {
                CardViewCreator.INSTANCE.CreateAt(
                    card,
                    deckPosition.position,
                    deckPosition.rotation
                );
            }
            ((ISplineCardHolder)this).UpdateCardPosition(
                CardViewRegistry.INSTANCE.GetAll(hand.Cards),
                transform.rotation
            );
        }
    }
}
