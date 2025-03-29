using System.Collections.Generic;
using gamecore.card;
using gamecore.game;
using UnityEngine;

namespace gameview
{
    public class CardViewCreator : MonoBehaviour
    {
        public static CardViewCreator INSTANCE { get; private set; }

        protected virtual void Awake()
        {
            if (INSTANCE != null)
            {
                Destroy(gameObject);
                return;
            }
            INSTANCE = this;
        }

        protected virtual void OnApplicationQuit()
        {
            INSTANCE = null;
            Destroy(gameObject);
        }

        [SerializeField]
        private CardView cardPrefab;

        public Dictionary<IPlayer, DiscardPileView> DiscardPileViews { get; } = new();

        public CardView CreateAt(ICard card, Vector3 position, Quaternion rotation)
        {
            var newCardView = Instantiate(cardPrefab, position, rotation);
            newCardView.Image.sprite = SpriteRegistry.INSTANCE.GetSprite(card.Id);
            newCardView.SetUp(DiscardPileViews[card.Owner].transform, card);
            CardViewRegistry.INSTANCE.Register(newCardView);
            return newCardView;
        }

        public CardView CreateIn(ICard card, Transform parent)
        {
            var newCardView = Instantiate(cardPrefab, parent);
            newCardView.Image.sprite = SpriteRegistry.INSTANCE.GetSprite(card.Id);
            newCardView.SetUp(DiscardPileViews[card.Owner].transform, card);
            return newCardView;
        }
    }
}
