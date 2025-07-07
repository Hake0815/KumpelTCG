using System;
using System.Collections.Generic;
using System.Linq;
using gamecore.card;
using UnityEngine;
using UnityEngine.EventSystems;

namespace gameview
{
    public class DiscardedCardsView : MonoBehaviour
    {
        [SerializeField]
        private Transform _scrollParent;

        [SerializeField]
        private RectTransform _scrollView;

        private readonly List<CardView> _cardViews = new();

        public void ShowCards(List<ICard> cards)
        {
            gameObject.SetActive(true);
            int numberOfRows = Mathf.CeilToInt((float)cards.Count / 5);
            var rectTransform = _scrollParent.GetComponent<RectTransform>();

            rectTransform.sizeDelta = new Vector2(
                rectTransform.sizeDelta.x,
                211.25f * numberOfRows
            );
            foreach (var card in cards)
            {
                var cardView = CardViewCreator.INSTANCE.CreateIn(card, _scrollParent);
                _cardViews.Add(cardView);
            }
            InputHandler.INSTANCE.OnMouseLeftClick += HideThis;
            InputHandler.INSTANCE.OnMouseRightClick += SetUpCardDetail;
        }

        private void SetUpCardDetail(Collider2D d)
        {
            var mousePos = Input.mousePosition;
            var cardView = _cardViews.FirstOrDefault(cardView =>
                RectTransformUtility.RectangleContainsScreenPoint(
                    cardView.GetComponent<RectTransform>(),
                    mousePos,
                    null
                )
            );
            cardView?.ShowCardDetail(new(-3f, 0f, -10.0f));
        }

        private void HideThis(Collider2D d)
        {
            if (!IsPointerOverUIObject(_scrollView))
            {
                ClearOldCards();
                InputHandler.INSTANCE.OnMouseLeftClick -= HideThis;
                InputHandler.INSTANCE.OnMouseRightClick -= SetUpCardDetail;
                gameObject.SetActive(false);
            }
        }

        private static bool IsPointerOverUIObject(RectTransform rect)
        {
            var mousePos = Input.mousePosition;
            return RectTransformUtility.RectangleContainsScreenPoint(rect, mousePos, null);
        }

        private void ClearOldCards()
        {
            foreach (var cardView in _cardViews)
            {
                Destroy(cardView.gameObject);
            }
            _cardViews.Clear();
        }
    }
}
