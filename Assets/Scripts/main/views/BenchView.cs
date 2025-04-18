using System;
using System.Collections.Generic;
using DG.Tweening;
using gamecore.card;
using gamecore.game;
using UnityEngine;

namespace gameview
{
    public class BenchView : MonoBehaviour, ICardDropArea
    {
        private IPlayer _player;
        private RectTransform _rectTransform;

        public void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void SetUp(IPlayer player)
        {
            _player = player;
            OnEnable();
        }

        private void OnEnable()
        {
            if (_player?.Bench != null)
            {
                _player.Bench.CardCountChanged += UpdateBenchedPokemonPositions;
            }
        }

        private void OnDisable()
        {
            if (_player.Bench != null)
            {
                _player.Bench.CardCountChanged -= UpdateBenchedPokemonPositions;
            }
        }

        public bool OnCardDropped(CardView cardView)
        {
            var card = cardView.Card;
            if (
                card is IPokemonCard pokemon
                && card.Owner == _player
                && pokemon.Stage == Stage.Basic
            )
            {
                Debug.Log("Valid Basic Pokemon dropped.");
                return true;
            }
            Debug.Log("Not a valid Basic Pokemon.");
            return false;
        }

        private void UpdateBenchedPokemonPositions()
        {
            var benchedPokemon = _player.Bench;
            var spacing = _rectTransform.rect.width / benchedPokemon.MaxBenchSpots;
            var orientation = _rectTransform.rotation * Vector3.right;
            var firstPosition =
                _rectTransform.position
                - (_rectTransform.rect.width - spacing) / 2f * orientation
                + Vector3.back;

            int i = 0;
            foreach (var pokemon in benchedPokemon)
            {
                var pokemonView = CardViewRegistry.INSTANCE.Get(pokemon);
                pokemonView.transform.DOMove(firstPosition + i * spacing * orientation, 0.25f);
                pokemonView.transform.DOLocalRotateQuaternion(_rectTransform.rotation, 0.25f);
                i++;
            }
        }
    }
}
