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

        [SerializeField]
        private TrainerCardView trainerCardPrefab;

        public Dictionary<IPlayer, DiscardPileView> DiscardPileViews { get; } = new();

        public CardView CreateAt(ICard card, Vector3 position, Quaternion rotation)
        {
            CardView newCardView;
            if (card is ITrainerCard)
            {
                var trainerCardView = Instantiate(trainerCardPrefab, position, rotation);
                trainerCardView.Image.sprite = SpriteRegistry.INSTANCE.GetSprite(card.Id);
                newCardView = trainerCardView;
            }
            else
            {
                newCardView = Instantiate(trainerCardPrefab, position, rotation);
            }
            newCardView.SetUp(DiscardPileViews[card.Owner].transform, card);
            CardViewRegistry.INSTANCE.Register(newCardView);
            return newCardView;
        }
    }
}
