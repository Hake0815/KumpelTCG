using System;
using System.Collections.Generic;
using DG.Tweening;
using gamecore.card;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace gameview
{
    public class CardDetailView : MonoBehaviour
    {
        private const float CARD_SPACING = 1f;
        private const float SCALE_FACTOR = 6.5f;
        private static readonly Vector3 DEFAULT_MIDDDLE_POSITION = new(0f, 0f, -10.0f);

        [SerializeField]
        private GameObject _cardHolder;

        [SerializeField]
        private Button _closeButton;
        private readonly List<CardView> _displayedCards = new();
        private readonly List<Collider2D> _cardViewColliders = new();
        private int _currentCardFocusIndex = 0;
        private int _totalCardCount = 0;
        private Vector3 _middlePosition = new(0f, 0f, -10.0f);

        private void Awake()
        {
            _closeButton.onClick.AddListener(CloseThis);
        }

        private void DecreaseCurrentId()
        {
            if (_currentCardFocusIndex > 0)
            {
                _currentCardFocusIndex--;
                UpdateView();
            }
        }

        private void IncreaseCurrentId()
        {
            if (_currentCardFocusIndex < _totalCardCount - 1)
            {
                _currentCardFocusIndex++;
                UpdateView();
            }
        }

        private void SetUpScrolling()
        {
            InputHandler.INSTANCE.OnMouseWheel += Scroll;
        }

        private void Scroll(float scroll)
        {
            if (scroll > 0)
                IncreaseCurrentId();
            else if (scroll < 0)
                DecreaseCurrentId();
        }

        public void DisplayCard(ICard cardToDisplay)
        {
            DisplayCard(cardToDisplay, DEFAULT_MIDDDLE_POSITION);
        }

        public void DisplayCard(ICard cardToDisplay, Vector3 middlePosition)
        {
            _middlePosition = middlePosition;
            gameObject.SetActive(true);
            _currentCardFocusIndex = 0;
            var cards = new List<ICard> { cardToDisplay };
            if (cardToDisplay is IPokemonCard pokemonCard)
            {
                cards.AddRange(pokemonCard.PreEvolutions);
                cards.AddRange(pokemonCard.AttachedEnergyCards);
            }
            foreach (var card in cards)
            {
                var cardView = CardViewCreator.INSTANCE.CreateIn(card, _cardHolder.transform);
                cardView.UnregisterCardDetailView();
                cardView.transform.localScale = Vector3.one * SCALE_FACTOR;
                _displayedCards.Add(cardView);
                _cardViewColliders.Add(cardView.GetComponent<Collider2D>());
            }
            _totalCardCount = _displayedCards.Count;
            UpdateView();
            SetUpScrolling();
            SetUpClose();
        }

        private void SetUpClose()
        {
            InputHandler.INSTANCE.OnMouseLeftClick += Close;
            InputHandler.INSTANCE.OnMouseRightClick += Close;
            InputHandler.INSTANCE.OnEsc += CloseThis;
        }

        private void Close(Collider2D clickedCollider)
        {
            if (_cardViewColliders.Contains(clickedCollider))
            {
                _currentCardFocusIndex = _cardViewColliders.IndexOf(clickedCollider);
                UpdateView();
                return;
            }
            if (
                _cardViewColliders.Contains(clickedCollider)
                || EventSystem.current.currentSelectedGameObject == _closeButton.gameObject
            )
                return;

            CloseThis();
        }

        private void UpdateView()
        {
            for (int i = 0; i < _totalCardCount; i++)
            {
                var xpos = (i - (_totalCardCount - 1) / 2f) * CARD_SPACING;
                var zoff = Math.Abs(i - _currentCardFocusIndex);
                _displayedCards[i].transform.position =
                    _middlePosition + Vector3.right * xpos + zoff * 0.01f * Vector3.forward;
                _displayedCards[i].Canvas.sortingOrder = 160 - zoff;
            }
        }

        private void CloseThis()
        {
            InputHandler.INSTANCE.OnMouseWheel -= Scroll;
            InputHandler.INSTANCE.OnMouseLeftClick -= Close;
            InputHandler.INSTANCE.OnMouseRightClick -= Close;
            InputHandler.INSTANCE.OnEsc -= CloseThis;
            Destroy(gameObject);
        }
    }
}
