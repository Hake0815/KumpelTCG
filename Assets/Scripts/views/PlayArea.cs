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
            if (player == cardView.Card.Owner && cardView.Card.IsPlayable())
            {
                cardView.transform.position = transform.position - new Vector3(0f, 0f, 0.01f);
                cardView.transform.rotation = transform.rotation;
                cardView.Card.PerformEffects();
                return true;
            }
            return false;
        }
    }
}
