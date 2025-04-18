using System;
using DG.Tweening;
using gamecore.game;
using UnityEngine;

namespace gameview
{
    public class ActiveSpot : MonoBehaviour, ICardDropArea
    {
        private IPlayer _player;
        private bool _isEmpty = true;

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
                _isEmpty = false;
                return true;
            }
            return false;
        }

        public void SetActivePokemon(CardView cardView)
        {
            cardView.transform.DOMove(transform.position + Vector3.back, 0.25f);
            cardView.transform.DORotateQuaternion(transform.rotation, 0.25f);
            _isEmpty = false;
        }
    }
}
