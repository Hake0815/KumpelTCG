using System.Collections.Generic;
using DG.Tweening;
using gamecore.card;
using UnityEngine;

namespace gameview
{
    public class FloatingSelectionView : MonoBehaviour
    {
        private static readonly float SCALE_FACTOR = 2f;
        private readonly List<CardView> _displayedCards = new();

        public void DisplayCards(List<ICard> cards)
        {
            _displayedCards.AddRange(CardViewRegistry.INSTANCE.GetAll(cards));
            var spacing =
                _displayedCards[0].GetComponent<RectTransform>().rect.width * 1.1f * SCALE_FACTOR;
            var firstPosition =
                (cards.Count - 1) / 2f * spacing * Vector3.left + 99f * Vector3.back;
            int i = 0;
            foreach (var cardView in _displayedCards)
            {
                cardView.transform.DOScale(SCALE_FACTOR, 0.25f);
                cardView.transform.DOMove(firstPosition + i * spacing * Vector3.right, 0.25f);
                i++;
            }
        }

        public void Clear()
        {
            foreach (var cardView in _displayedCards)
            {
                cardView.transform.DOScale(Vector3.one, 0.25f);
            }
            _displayedCards.Clear();
        }
    }
}
