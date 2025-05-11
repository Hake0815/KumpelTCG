using System;
using DG.Tweening;
using gamecore.card;
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
            _player.ActivePokemonSet += SetActivePokemon;
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

        private void SetActivePokemon(IPokemonCard card)
        {
            Debug.Log("Set active pokemon called");
            Debug.Log($"Setting active pokemon: {card.Name}");
            SetActivePokemon(CardViewRegistry.INSTANCE.Get(card));
        }

        public void SetActivePokemon(CardView cardView)
        {
            UIQueue.INSTANCE.Queue(
                (callback) =>
                {
                    DOTween
                        .Sequence()
                        .Join(cardView.transform.DOMove(transform.position + Vector3.back, 0.25f))
                        .Join(cardView.transform.DORotateQuaternion(transform.rotation, 0.25f));
                    callback.Invoke();
                }
            );
            _isEmpty = false;
        }
    }
}
