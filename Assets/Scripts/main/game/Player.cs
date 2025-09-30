using System;
using System.Collections.Generic;
using gamecore.card;
using gamecore.effect;
using Newtonsoft.Json;
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
        Dictionary<Type, IPlayerEffect> PlayerEffects { get; }
        event Action<IPokemonCard> ActivePokemonSet;
    }

    internal interface IPlayerLogic : IPlayer
    {
        new string Name { get; set; }

        [JsonIgnore]
        new bool IsActive { get; set; }

        [JsonIgnore]
        new IPokemonCardLogic ActivePokemon { get; set; }

        [JsonIgnore]
        IPokemonCard IPlayer.ActivePokemon => ActivePokemon;

        [JsonIgnore]
        new DeckLogicAbstract Deck { get; set; }
        IDeck IPlayer.Deck => Deck;

        [JsonIgnore]
        IDeckList DeckList { get; }

        [JsonIgnore]
        new HandLogicAbstract Hand { get; }
        IHand IPlayer.Hand => Hand;

        [JsonIgnore]
        new BenchLogicAbstract Bench { get; }
        IBench IPlayer.Bench => Bench;

        [JsonIgnore]
        new PrizesLogicAbstract Prizes { get; }
        IPrizes IPlayer.Prizes => Prizes;

        [JsonIgnore]
        HashSet<string> PerformedOncePerTurnActions { get; }

        [JsonIgnore]
        new DiscardPileLogicAbstract DiscardPile { get; }
        IDiscardPile IPlayer.DiscardPile => DiscardPile;

        [JsonIgnore]
        IPlayerLogic Opponent { get; }

        [JsonIgnore]
        int TurnCounter { get; set; }
        List<ICardLogic> Draw(int amount);
        void SetPrizeCards();
        void ResetOncePerTurnActions();
        void Promote(IPokemonCardLogic pokemon);
        bool HasEffect<T>()
            where T : IPlayerEffect;
        void AddEffect(IPlayerEffect effect);
        void RemoveEffect(IPlayerEffect effect);
    }

    [JsonObject(MemberSerialization.OptIn)]
    class Player : IPlayerLogic
    {
        private bool _isActive;

        [JsonProperty]
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
                {
                    _activePokemon.OwnerPositionKnowledge = PositionKnowledge.Known;
                    _activePokemon.OpponentPositionKnowledge = PositionKnowledge.Known;
                    ActivePokemonSet?.Invoke(value);
                }
            }
        }
        public IDeckList DeckList { get; set; }
        public DeckLogicAbstract Deck { get; set; }
        public HandLogicAbstract Hand { get; } = new Hand();
        public BenchLogicAbstract Bench { get; } = new Bench();
        public DiscardPileLogicAbstract DiscardPile { get; } = new DiscardPile();
        public PrizesLogicAbstract Prizes { get; } = new Prizes();
        public HashSet<string> PerformedOncePerTurnActions { get; } = new();
        public IPlayerLogic Opponent { get; set; }
        public int TurnCounter { get; set; } = 0;
        public Dictionary<Type, IPlayerEffect> PlayerEffects { get; } = new();

        public event Action<IPokemonCard> ActivePokemonSet;

        public List<ICardLogic> Draw(int amount)
        {
            var drawnCards = Deck.Draw(amount);
            if (drawnCards != null)
            {
                Hand.AddCards(drawnCards);
            }
            return drawnCards;
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

        public bool HasEffect<T>()
            where T : IPlayerEffect
        {
            return PlayerEffects.ContainsKey(typeof(T));
        }

        public void AddEffect(IPlayerEffect effect)
        {
            PlayerEffects[effect.GetType()] = effect;
        }

        public void RemoveEffect(IPlayerEffect effect)
        {
            PlayerEffects.Remove(effect.GetType());
        }
    }
}
