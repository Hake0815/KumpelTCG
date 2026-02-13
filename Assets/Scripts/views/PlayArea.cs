using gamecore.card;
using gamecore.game;
using UnityEngine;

namespace gameview
{
    public class PlayArea : MonoBehaviour, ICardDropArea
    {
        private IPlayer _player;

        public void SetUp(IPlayer player)
        {
            _player = player;
        }

        public bool OnCardDropped(CardView cardView)
        {
            var card = cardView.Card;
            if (card.IsTrainerCard() && card.Owner == _player)
            {
                cardView.transform.SetPositionAndRotation(transform.position, transform.rotation);
                return true;
            }
            return false;
        }
    }
}
