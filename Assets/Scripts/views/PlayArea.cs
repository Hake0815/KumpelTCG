using gamecore.card;
using gamecore.game;
using UnityEngine;

namespace gameview
{
    public class PlayArea : MonoBehaviour, ICardDropArea
    {
        private IPlayer player;

        public void SetUp(IPlayer player)
        {
            this.player = player;
        }

        public bool OnCardDropped(CardView cardView)
        {
            var card = cardView.Card;
            if (card.IsTrainerCard() && card.Owner == player && card.IsPlayable())
            {
                cardView.transform.position = transform.position;
                cardView.transform.rotation = transform.rotation;
                card.Play();
                return true;
            }
            return false;
        }
    }
}
