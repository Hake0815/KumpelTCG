using System;
using System.Collections.Generic;
using System.Linq;
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
        private IDiscardPile _discardPile;
        private Image _image;
        public TMP_Text Text { get; private set; }

        private void Awake()
        {
            _image = transform.Find("Sprite").GetComponent<Image>();
            Text = GetComponentInChildren<TMP_Text>();
        }

        public void SetUp(IDiscardPile discardPile)
        {
            _discardPile = discardPile;
            OnEnable();
        }

        public void UpdateView()
        {
            UpdateView(_discardPile.Cards);
        }

        private void OnEnable()
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
            Text.text = cards.Count.ToString();
        }

        private void DiscardCards(List<ICard> cards)
        {
            var cardViews = CardViewRegistry.INSTANCE.GetAll(cards);
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

        private void CreateCards(List<ICard> cards)
        {
            UIQueue.INSTANCE.Queue(CallbackOnDone =>
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
                CallbackOnDone.Invoke();
            });
        }
    }
}
