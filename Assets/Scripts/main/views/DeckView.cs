using System;
using System.Collections.Generic;
using gamecore.card;
using gamecore.game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace gameview
{
    public class DeckView : MonoBehaviour
    {
        public IDeck Deck { get; private set; }
        private TMP_Text _text;
        private Image _image;

        private Sprite _sprite;
        private Sprite _emptySprite;

        private void Awake()
        {
            _image = transform.Find("Sprite").GetComponent<Image>();
            _text = GetComponentInChildren<TMP_Text>();
            _sprite = Resources.Load<Sprite>("Images/back_side");
            _emptySprite = Resources.Load<Sprite>("Images/back_side_faded");
        }

        public void SetUp(IPlayer player)
        {
            Deck = player.Deck;
            _text = GetComponentInChildren<TMP_Text>();
            _text.text = Deck.CardCount.ToString();
            OnEnable();
        }

        private void OnEnable()
        {
            if (Deck != null)
            {
                Deck.CardCountChanged += UpdateView;
                Deck.CardsDrawn += OnCardsDrawn;
                Deck.CardsDrawnFaceDown += OnCardsDrawnFaceDown;
            }
        }

        private void OnDisable()
        {
            if (Deck != null)
            {
                Deck.CardCountChanged -= UpdateView;
                Deck.CardsDrawn -= OnCardsDrawn;
                Deck.CardsDrawnFaceDown -= OnCardsDrawnFaceDown;
            }
        }

        public void UpdateView()
        {
            _text.text = Deck.CardCount.ToString();
            if (Deck.CardCount == 0)
                _image.sprite = _emptySprite;
            else
                _image.sprite = _sprite;
        }

        private void OnCardsDrawn(object sender, List<ICard> drawnCards)
        {
            CreateDrawnCards(drawnCards);
        }

        public void CreateDrawnCards(List<ICard> drawnCards)
        {
            UIQueue.INSTANCE.Queue(CallbackOnDone =>
            {
                foreach (var card in drawnCards)
                {
                    CardViewCreator.INSTANCE.CreateAt(card, transform.position, transform.rotation);
                }
                CallbackOnDone.Invoke();
            });
        }

        private void OnCardsDrawnFaceDown(object sender, List<ICard> drawnCards)
        {
            CreateDrawnCardsFaceDown(drawnCards);
        }

        public void CreateDrawnCardsFaceDown(List<ICard> drawnCards)
        {
            UIQueue.INSTANCE.Queue(CallbackOnDone =>
            {
                foreach (var card in drawnCards)
                {
                    CardViewCreator.INSTANCE.CreateAtFaceDown(
                        card,
                        transform.position,
                        transform.rotation
                    );
                }
                CallbackOnDone.Invoke();
            });
        }
    }
}
