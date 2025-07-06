using System;
using System.Collections.Generic;
using gamecore.card;
using UnityEngine;

namespace gameview
{
    public class DiscardedCardsView : MonoBehaviour
    {
        [SerializeField]
        private Transform _scrollParent;

        [SerializeField]
        private RectTransform _scrollView;

        public void ShowCards(List<ICard> cards)
        {
            Debug.Log($"Showing {cards.Count} cards");
            gameObject.SetActive(true);
            int numberOfRows = Mathf.CeilToInt((float)cards.Count / 5);
            var rectTransform = _scrollParent.GetComponent<RectTransform>();

            rectTransform.sizeDelta = new Vector2(
                rectTransform.sizeDelta.x,
                211.25f * numberOfRows
            );
            foreach (var card in cards)
            {
                CardViewCreator.INSTANCE.CreateIn(card, _scrollParent);
            }
            InputHandler.INSTANCE.OnMouseLeftClick += HideThis;
        }

        private void HideThis(Collider2D d)
        {
            if (!IsPointerOverUIObject(_scrollView))
            {
                ClearOldCards();
                InputHandler.INSTANCE.OnMouseLeftClick -= HideThis;
                gameObject.SetActive(false);
            }
        }

        bool IsPointerOverUIObject(RectTransform rect)
        {
            var mousePos = Input.mousePosition;
            return RectTransformUtility.RectangleContainsScreenPoint(rect, mousePos, null);
        }

        private void ClearOldCards()
        {
            foreach (Transform child in _scrollParent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
