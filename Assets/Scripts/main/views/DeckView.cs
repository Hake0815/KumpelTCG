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
            _text.text = Deck.GetCardCount().ToString();
            OnEnable();
        }

        private void OnEnable()
        {
            if (Deck != null)
                Deck.CardCountChanged += UpdateView;
        }

        private void OnDisable()
        {
            if (Deck != null)
                Deck.CardCountChanged -= UpdateView;
        }

        public void UpdateView()
        {
            _text.text = Deck.GetCardCount().ToString();
            if (Deck.GetCardCount() == 0)
                _image.sprite = _emptySprite;
            else
                _image.sprite = _sprite;
        }
    }
}
