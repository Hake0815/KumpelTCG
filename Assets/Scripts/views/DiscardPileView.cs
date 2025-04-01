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
            this._discardPile = discardPile;
            OnEnable();
        }

        private void OnEnable()
        {
            if (_discardPile != null)
                _discardPile.CardsChanged += OnCardsChanged;
        }

        private void OnCardsChanged()
        {
            UpdateView();
        }

        private void OnDisable()
        {
            if (_discardPile != null)
                _discardPile.CardsChanged -= OnCardsChanged;
        }

        private void UpdateView()
        {
            var topCard = _discardPile.GetLastCard();
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
            UpdateText();
        }

        private void UpdateText()
        {
            Text.text = _discardPile.GetCardCount().ToString();
        }
    }
}
