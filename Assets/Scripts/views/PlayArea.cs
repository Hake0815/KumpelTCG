using UnityEngine;

namespace gameview
{
    public class PlayArea : MonoBehaviour, ICardDropArea
    {
        public void OnCardDropped(CardView cardView)
        {
            cardView.transform.position = transform.position - new Vector3(0f, 0f, 0.01f);
            cardView.transform.rotation = transform.rotation;
            cardView.Card.PerformEffects();
        }
    }
}
