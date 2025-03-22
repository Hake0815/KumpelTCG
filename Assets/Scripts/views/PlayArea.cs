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
            if (
                cardView.Card is ITrainerCard trainerCard
                && trainerCard.Owner == player
                && trainerCard.IsPlayable()
            )
            {
                cardView.transform.position = transform.position;
                cardView.transform.rotation = transform.rotation;
                trainerCard.Play();
                return true;
            }
            return false;
        }
    }
}
