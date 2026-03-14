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
        private CardView _cardPrefab;

        public CardView CreateAtFaceDown(ICard card, Vector3 position, Quaternion rotation)
        {
            var newCardView = CreateAt(card, position, rotation);
            newCardView.FaceUp = false;
            return newCardView;
        }

        public CardView CreateAt(ICard card, Vector3 position, Quaternion rotation)
        {
            var newCardView = Instantiate(_cardPrefab, position, rotation);
            newCardView.SetUp(
                card,
                SpriteRegistry.INSTANCE.GetSprite(card.Id),
                SpriteRegistry.INSTANCE.GetIconSprite(card.Id)
            );
            CardViewRegistry.INSTANCE.Register(newCardView);
            return newCardView;
        }

        public CardView CreateIn(ICard card, Transform parent)
        {
            var newCardView = Instantiate(_cardPrefab, parent);
            newCardView.SetUp(
                card,
                SpriteRegistry.INSTANCE.GetSprite(card.Id),
                SpriteRegistry.INSTANCE.GetIconSprite(card.Id)
            );
            return newCardView;
        }
    }
}
