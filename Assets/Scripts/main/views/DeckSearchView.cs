using System;
using System.Collections.Generic;
using DG.Tweening;
using gamecore.card;
using gamecore.game;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace gameview
{
    public class DeckSearchView : MonoBehaviour
    {
        private static readonly string SHOW_CARDS_TEXT = "Show cards";
        private static readonly string HIDE_CARDS_TEXT = "Hide cards";
        private const float WIDTH = 6f;
        private const float HEIGHT = 2.5f;
        private const float CARD_SPACING = 1f;
        private readonly Vector3 MIDDLE_POSITION = new(0.0f, HEIGHT, -10.0f);

        [SerializeField]
        private GameObject _cardHolder;

        [SerializeField]
        private Button _toggleCardsButton;

        [SerializeField]
        private Button _increaseCurrentIdButton;

        [SerializeField]
        private Button _decreaseCurrentIdButton;

        [SerializeField]
        private TMP_Text _buttonText;
        private static readonly float SCALE_FACTOR = 2f;
        private readonly List<CardView> _displayedCards = new();
        private readonly List<Collider2D> _cardViewColliders = new();
        private int _currentCardFocusIndex = 0;
        private int _totalCardCount = 0;

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

        private void Awake()
        {
            _decreaseCurrentIdButton.onClick.AddListener(DecreaseCurrentId);
            _increaseCurrentIdButton.onClick.AddListener(IncreaseCurrentId);
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

        public void DisplayCards(IDeck deck)
        {
            gameObject.SetActive(true);
            _currentCardFocusIndex = 0;
            foreach (var card in deck.Cards)
            {
                var cardView = CardViewCreator.INSTANCE.CreateAt(
                    card,
                    transform.position,
                    transform.rotation
                );
                cardView.transform.localScale = Vector3.one * SCALE_FACTOR;
                cardView.transform.SetParent(_cardHolder.transform);
                _displayedCards.Add(cardView);
                _cardViewColliders.Add(cardView.GetComponent<Collider2D>());
            }
            _displayedCards.Sort(CardViewComparer.Create());
            _totalCardCount = _displayedCards.Count;
            UpdateView();
            SetUpHide();
            SetUpScrolling();
            Hidden = false;
            _toggleCardsButton.onClick.RemoveAllListeners();
            _toggleCardsButton.onClick.AddListener(() =>
            {
                Hidden = !Hidden;
            });
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

        private void UpdateView()
        {
            _increaseCurrentIdButton.interactable = _currentCardFocusIndex < _totalCardCount - 1;
            _decreaseCurrentIdButton.interactable = _currentCardFocusIndex > 0;
            ArangeCards();
        }

        private void ArangeCards()
        {
            for (int i = 0; i < _totalCardCount; i++)
            {
                var xpos = (i - _currentCardFocusIndex) * CARD_SPACING;
                var zoff = Math.Abs(i - _currentCardFocusIndex);
                _displayedCards[i]
                    .transform.DOMove(
                        MIDDLE_POSITION + Vector3.right * xpos + zoff * 0.01f * Vector3.forward,
                        AnimationSpeedHolder.AnimationSpeed
                    );
                _displayedCards[i].Canvas.sortingOrder = 160 - zoff;
            }
        }

        private void ArangeCardsAfterCurrentIndex()
        {
            for (int i = _currentCardFocusIndex; i < _totalCardCount; i++)
            {
                var xpos =
                    (float)
                        Math.Log(
                            i - _currentCardFocusIndex + 1,
                            _totalCardCount - _currentCardFocusIndex + 1
                        ) * WIDTH;
                _displayedCards[i]
                    .transform.DOMove(
                        MIDDLE_POSITION + Vector3.right * xpos,
                        AnimationSpeedHolder.AnimationSpeed
                    );
            }
        }

        private void SetUpHide()
        {
            InputHandler.INSTANCE.OnMouseLeftClick += HideThis;
            InputHandler.INSTANCE.SkipOneFrame();
        }

        private void HideThis(Collider2D clickedCollider)
        {
            if (
                _cardViewColliders.Contains(clickedCollider)
                || EventSystem.current.currentSelectedGameObject == _toggleCardsButton.gameObject
                || EventSystem.current.currentSelectedGameObject
                    == _increaseCurrentIdButton.gameObject
                || EventSystem.current.currentSelectedGameObject
                    == _decreaseCurrentIdButton.gameObject
            )
                return;
            Hidden = true;
        }

        public void Clear()
        {
            Debug.Log("Clearing cards");
            foreach (var cardView in _displayedCards)
            {
                CardViewRegistry.INSTANCE.Unregister(cardView.Card);
                Destroy(cardView.gameObject);
            }
            _displayedCards.Clear();
            gameObject.SetActive(false);
            InputHandler.INSTANCE.OnMouseLeftClick -= HideThis;
            InputHandler.INSTANCE.OnMouseWheel -= Scroll;
            _toggleCardsButton.onClick.RemoveAllListeners();
        }
    }
}
