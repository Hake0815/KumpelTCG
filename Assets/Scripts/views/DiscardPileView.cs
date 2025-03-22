using gamecore.card;
using gamecore.game;
using TMPro;
using UnityEngine;

namespace gameview
{
    public class DiscardPileView : MonoBehaviour
    {
        private IDiscardPile discardPile;
        private SpriteRenderer spriteRenderer;
        public TMP_Text Text { get; private set; }

        public void SetUp(IDiscardPile discardPile)
        {
            this.discardPile = discardPile;
            spriteRenderer = GetComponent<SpriteRenderer>();
            Text = GetComponentInChildren<TMP_Text>();
            OnEnable();
        }

        private void OnEnable()
        {
            if (discardPile != null)
                discardPile.CardsChanged += OnCardsChanged;
        }

        private void OnCardsChanged()
        {
            UpdateView();
        }

        private void OnDisable()
        {
            if (discardPile != null)
                discardPile.CardsChanged -= OnCardsChanged;
        }

        private void UpdateView()
        {
            Debug.Log("Updating discard pile view");
            var topCard = discardPile.GetLastCard();
            if (topCard != null)
                spriteRenderer.sprite = CardViewCreator.INSTANCE.GetSprite(topCard);
            else
                spriteRenderer.sprite = null;
            UpdateText();
        }

        private void UpdateText()
        {
            Text.text = discardPile.GetCardCount().ToString();
        }
    }
}
