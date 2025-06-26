using System;
using System.Collections.Generic;
using DG.Tweening;
using gamecore.card;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace gameview
{
    public class FloatingSelectionView : MonoBehaviour
    {
        private static readonly string SHOW_CARDS_TEXT = "Show cards";
        private static readonly string HIDE_CARDS_TEXT = "Hide cards";

        [SerializeField]
        private GameObject _cardHolder;

        [SerializeField]
        private Button _toggleCardsButton;

        [SerializeField]
        private TMP_Text _buttonText;
        private static readonly float SCALE_FACTOR = 2f;
        private readonly List<CardView> _displayedCards = new();
        private readonly List<Collider2D> _cardViewColliders = new();
        private bool _hidden;
        private bool Hidden
        {
            get => _hidden;
            set
            {
                _hidden = value;
                _cardHolder.gameObject.SetActive(!_hidden);
                SetButtonText();
            }
        }

        private void SetButtonText()
        {
            if (Hidden)
                _buttonText.text = SHOW_CARDS_TEXT;
            else
                _buttonText.text = HIDE_CARDS_TEXT;
        }

        public void DisplayCards(List<ICard> cards)
        {
            gameObject.SetActive(true);
            _displayedCards.AddRange(CardViewRegistry.INSTANCE.GetAll(cards));
            var spacing =
                _displayedCards[0].GetComponent<RectTransform>().rect.width * 1.1f * SCALE_FACTOR;
            var firstPosition =
                (cards.Count - 1) / 2f * spacing * Vector3.left + 99f * Vector3.back;
            int i = 0;
            foreach (var cardView in _displayedCards)
            {
                _cardViewColliders.Add(cardView.GetComponent<Collider2D>());
                cardView.transform.SetParent(_cardHolder.transform);
                cardView.transform.DOScale(SCALE_FACTOR, AnimationSpeedHolder.AnimationSpeed);
                cardView.transform.DOMove(
                    firstPosition + i * spacing * Vector3.right,
                    AnimationSpeedHolder.AnimationSpeed
                );
                i++;
            }
            Hidden = false;
            SetUpHide();
            _toggleCardsButton.onClick.RemoveAllListeners();
            _toggleCardsButton.onClick.AddListener(() =>
            {
                Hidden = !Hidden;
            });
        }

        private void SetUpHide()
        {
            InputHandler.INSTANCE.OnMouseLeftClick += HideThis;
        }

        private void HideThis(Collider2D clickedCollider)
        {
            if (
                _cardViewColliders.Contains(clickedCollider)
                || EventSystem.current.currentSelectedGameObject == _toggleCardsButton.gameObject
            )
                return;
            Hidden = true;
        }

        public void Clear()
        {
            foreach (var cardView in _displayedCards)
            {
                cardView.transform.SetParent(null);
                cardView.transform.DOScale(Vector3.one, AnimationSpeedHolder.AnimationSpeed);
            }
            _displayedCards.Clear();
            gameObject.SetActive(false);
            InputHandler.INSTANCE.OnMouseLeftClick -= HideThis;
            _toggleCardsButton.onClick.RemoveAllListeners();
        }
    }
}
