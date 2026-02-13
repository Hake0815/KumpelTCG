using System;
using System.Threading.Tasks;
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
            _player.ActivePokemonSet += OnSetActivePokemon;
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

        private async void OnSetActivePokemon(ICard card)
        {
            await SetActivePokemon(card);
        }

        public async Task SetActivePokemon(ICard card)
        {
            await UIQueue.INSTANCE.Queue(() =>
            {
                var cardView = CardViewRegistry.INSTANCE.Get(card);
                DOTween
                    .Sequence()
                    .Join(
                        cardView.transform.DOMove(
                            transform.position + Vector3.back,
                            AnimationSpeedHolder.AnimationSpeed
                        )
                    )
                    .Join(
                        cardView.transform.DORotateQuaternion(
                            transform.rotation,
                            AnimationSpeedHolder.AnimationSpeed
                        )
                    );
                return Task.CompletedTask;
            });
            _isEmpty = false;
        }
    }
}
