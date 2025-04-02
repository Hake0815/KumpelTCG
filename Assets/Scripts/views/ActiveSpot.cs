using System;
using gamecore.game;
using UnityEngine;

namespace gameview
{
    public class ActiveSpot : MonoBehaviour, ICardDropArea
    {
        private IPlayer _player;
        private bool _isEmpty = true;

        public event Action CardPlayed;

        public void SetUp(IPlayer player)
        {
            _player = player;
        }

        public bool OnCardDropped(CardView cardView)
        {
            var card = cardView.Card;
            if (card.IsPokemonCard() && card.Owner == _player && _isEmpty)
            {
                cardView.transform.position = transform.position;
                cardView.transform.rotation = transform.rotation;
                card.Play();
                _isEmpty = false;
                Debug.Log($"ActiveSpot: CardPlayed");
                CardPlayed?.Invoke();
                return true;
            }
            return false;
        }
    }
}
