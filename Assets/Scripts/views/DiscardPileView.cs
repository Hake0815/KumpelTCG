using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using gamecore.card;
using gamecore.game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace gameview
{
    public class DiscardPileView : MonoBehaviour
    {
        [SerializeField]
        private DiscardedCardsView _discardedCardsViewPrefab;
        private DiscardedCardsView _discardedCardsView;
        private Collider2D _collider;
        private IDiscardPile _discardPile;
        private Image _image;
        public TMP_Text _text;

        private void Awake()
        {
            _image = transform.Find("Sprite").GetComponent<Image>();
            _text = GetComponentInChildren<TMP_Text>();
            _collider = GetComponent<Collider2D>();
        }

        public void SetUp(IDiscardPile discardPile)
        {
            _discardPile = discardPile;
            _discardedCardsView = Instantiate(_discardedCardsViewPrefab);
            _discardedCardsView.gameObject.SetActive(false);
            RegisterOnDiscardPileEvents();
        }

        public void UpdateView()
        {
            UpdateView(_discardPile.Cards);
        }

        private void OnEnable()
        {
            RegisterOnDiscardPileEvents();
            InputHandler.INSTANCE.OnMouseLeftClick += ShowDiscardedCards;
        }

        private void RegisterOnDiscardPileEvents()
        {
            if (_discardPile != null)
            {
                _discardPile.CardCountChanged += UpdateView;
                _discardPile.CardsAdded += DiscardCards;
                _discardPile.CardsRemoved += CreateCards;
            }
        }

        private void OnDisable()
        {
            if (_discardPile != null)
            {
                _discardPile.CardCountChanged -= UpdateView;
                _discardPile.CardsAdded -= DiscardCards;
                _discardPile.CardsRemoved -= CreateCards;
            }
            if (InputHandler.INSTANCE is not null)
            {
                InputHandler.INSTANCE.OnMouseLeftClick -= ShowDiscardedCards;
            }
        }

        private void UpdateView(List<ICard> cards)
        {
            var topCard = cards.LastOrDefault();
            if (topCard != null)
            {
                _image.sprite = SpriteRegistry.INSTANCE.GetSprite(topCard.Id);
                _image.color = new Color(1, 1, 1, 1);
            }
            else
            {
                _image.sprite = null;
                _image.color = new Color(1, 1, 1, 0f);
            }
            _text.text = cards.Count.ToString();
        }

        private void DiscardCards(List<ICard> cards)
        {
            var cardViews = CardViewRegistry.INSTANCE.GetAllAvailable(cards);
            foreach (var cardView in cardViews)
            {
                CardViewRegistry.INSTANCE.Unregister(cardView.Card);
                var cardViewTransform = cardView.GetComponent<Transform>();
                DOTween
                    .Sequence()
                    .Append(
                        cardViewTransform.DOMove(
                            transform.position,
                            AnimationSpeedHolder.AnimationSpeed
                        )
                    )
                    .Join(
                        cardViewTransform.DORotateQuaternion(
                            transform.rotation,
                            AnimationSpeedHolder.AnimationSpeed
                        )
                    )
                    .OnComplete(() => Destroy(cardView.gameObject));
            }
        }

        private async void CreateCards(List<ICard> cards)
        {
            await UIQueue.INSTANCE.Queue(() =>
            {
                foreach (var card in cards)
                {
                    var cardView = CardViewCreator.INSTANCE.CreateAt(
                        card,
                        transform.position,
                        transform.rotation
                    );
                    cardView.Canvas.sortingOrder = card.Owner.Hand.Cards.Count + 1;
                }
                return Task.CompletedTask;
            });
        }

        private void ShowDiscardedCards(Collider2D d)
        {
            if (_discardPile.Cards.Count > 0 && _collider.Equals(d))
                _discardedCardsView.ShowCards(_discardPile.Cards);
        }
    }
}
