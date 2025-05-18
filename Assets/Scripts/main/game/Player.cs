using System;
using System.Collections.Generic;
using gamecore.card;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering.VirtualTexturing;

namespace gamecore.game
{
    public interface IPlayer
    {
        string Name { get; }
        IDeck Deck { get; }
        IHand Hand { get; }
        IBench Bench { get; }
        IDiscardPile DiscardPile { get; }
        IPrizes Prizes { get; }
        bool IsActive { get; }
        IPokemonCard ActivePokemon { get; }
        event Action<IPokemonCard> ActivePokemonSet;
    }

    internal interface IPlayerLogic : IPlayer
    {
        new string Name { get; set; }
        new bool IsActive { get; set; }
        new IPokemonCardLogic ActivePokemon { get; set; }
        IPokemonCard IPlayer.ActivePokemon => ActivePokemon;
        new IDeckLogic Deck { get; set; }
        IDeck IPlayer.Deck => Deck;
        new IHandLogic Hand { get; }
        IHand IPlayer.Hand => Hand;
        new IBenchLogic Bench { get; }
        IBench IPlayer.Bench => Bench;
        new IPrizesLogic Prizes { get; }
        IPrizes IPlayer.Prizes => Prizes;
        HashSet<string> PerformedOncePerTurnActions { get; }

        new IDiscardPileLogic DiscardPile { get; }
        IDiscardPile IPlayer.DiscardPile => DiscardPile;
        IPlayerLogic Opponent { get; }
        int TurnCounter { get; set; }
        void Draw(int amount);
        void SetPrizeCards();
        void ResetOncePerTurnActions();
        void Promote(IPokemonCardLogic pokemon);
    }

    class Player : IPlayerLogic
    {
        private bool _isActive;
        public string Name { get; set; }
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                if (!value)
                    ResetOncePerTurnActions();
            }
        }
        private IPokemonCardLogic _activePokemon;
        public IPokemonCardLogic ActivePokemon
        {
            get => _activePokemon;
            set
            {
                _activePokemon = value;
                if (value != null)
                    ActivePokemonSet?.Invoke(value);
            }
        }
        public IDeckLogic Deck { get; set; }
        public IHandLogic Hand { get; } = new Hand();
        public IBenchLogic Bench { get; } = new Bench();
        public IDiscardPileLogic DiscardPile { get; } = new DiscardPile();
        public IPrizesLogic Prizes { get; } = new Prizes();
        public HashSet<string> PerformedOncePerTurnActions { get; } = new();
        public IPlayerLogic Opponent { get; set; }
        public int TurnCounter { get; set; } = 0;

        public event Action<IPokemonCard> ActivePokemonSet;

        public void Draw(int amount)
        {
            var drawnCards = Deck.Draw(amount);
            if (drawnCards != null)
            {
                Hand.AddCards(drawnCards);
            }
        }

        public void Promote(IPokemonCardLogic pokemon)
        {
            Bench.RemoveCard(pokemon);
            ActivePokemon = pokemon;
        }

        public void ResetOncePerTurnActions()
        {
            PerformedOncePerTurnActions.Clear();
        }

        public void SetPrizeCards()
        {
            var prizeCards = Deck.DrawFaceDown(6);
            Prizes.AddCards(prizeCards);
        }
    }
}
