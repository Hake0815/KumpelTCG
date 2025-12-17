using System;
using System.Collections.Generic;
using System.Linq;
using gamecore.card;
using gamecore.effect;
using gamecore.serialization;
using Newtonsoft.Json;

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
        bool IsAttacking { get; }
        IPokemonCard ActivePokemon { get; }
        ICard CurrentlyPlayedCard { get; }
        List<ICard> FloatingCards { get; }
        Dictionary<Type, PlayerEffectAbstract> PlayerEffects { get; }
        IPlayer Opponent { get; }
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
        new ICardLogic CurrentlyPlayedCard { get; set; }

        [JsonIgnore]
        ICard IPlayer.CurrentlyPlayedCard => CurrentlyPlayedCard;

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
        new List<ICardLogic> FloatingCards { get; set; }
        List<ICard> IPlayer.FloatingCards => FloatingCards?.Cast<ICard>().ToList();

        [JsonIgnore]
        HashSet<OncePerTurnActionType> PerformedOncePerTurnActions { get; }

        [JsonIgnore]
        new DiscardPileLogicAbstract DiscardPile { get; }
        IDiscardPile IPlayer.DiscardPile => DiscardPile;

        [JsonIgnore]
        new IPlayerLogic Opponent { get; }
        IPlayer IPlayer.Opponent => Opponent;

        [JsonIgnore]
        int TurnCounter { get; set; }

        [JsonIgnore]
        new bool IsAttacking { get; set; }
        bool IPlayer.IsAttacking => IsAttacking;
        List<ICardLogic> Draw(int amount);
        void SetPrizeCards();
        void ResetOncePerTurnActions();
        void Promote(IPokemonCardLogic pokemon);
        bool HasEffect<T>()
            where T : PlayerEffectAbstract;
        void AddEffect(PlayerEffectAbstract effect);
        void RemoveEffect(PlayerEffectAbstract effect);
        PlayerStateJson ToSerializable();
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
        public bool IsAttacking { get; set; }
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
        private ICardLogic _currentlyPlayedCard;
        public ICardLogic CurrentlyPlayedCard
        {
            get => _currentlyPlayedCard;
            set
            {
                _currentlyPlayedCard = value;
                if (value != null)
                {
                    _currentlyPlayedCard.OwnerPositionKnowledge = PositionKnowledge.Known;
                    _currentlyPlayedCard.OpponentPositionKnowledge = PositionKnowledge.Known;
                }
            }
        }
        private List<ICardLogic> _floatingCards;
        public List<ICardLogic> FloatingCards
        {
            get => _floatingCards;
            set
            {
                _floatingCards = value;
                if (value != null)
                {
                    foreach (var card in value)
                    {
                        card.OwnerPositionKnowledge = PositionKnowledge.Known;
                    }
                }
            }
        }
        public IDeckList DeckList { get; set; }
        public DeckLogicAbstract Deck { get; set; }
        public HandLogicAbstract Hand { get; } = new Hand();
        public BenchLogicAbstract Bench { get; } = new Bench();
        public DiscardPileLogicAbstract DiscardPile { get; } = new DiscardPile();
        public PrizesLogicAbstract Prizes { get; } = new Prizes();
        public HashSet<OncePerTurnActionType> PerformedOncePerTurnActions { get; } = new();
        public IPlayerLogic Opponent { get; set; }
        public int TurnCounter { get; set; } = 0;
        public Dictionary<Type, PlayerEffectAbstract> PlayerEffects { get; } = new();

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
            where T : PlayerEffectAbstract
        {
            return PlayerEffects.ContainsKey(typeof(T));
        }

        public void AddEffect(PlayerEffectAbstract effect)
        {
            PlayerEffects[effect.GetType()] = effect;
        }

        public void RemoveEffect(PlayerEffectAbstract effect)
        {
            PlayerEffects.Remove(effect.GetType());
        }

        public PlayerStateJson ToSerializable()
        {
            var playerEffects = new List<PlayerEffectType>();
            foreach (var effect in PlayerEffects.Values)
            {
                playerEffects.Add(effect.ToSerializable());
            }
            return new PlayerStateJson(
                isActive: IsActive,
                isAttacking: IsAttacking,
                handCount: Hand.CardCount,
                deckCount: Deck.CardCount,
                prizesCount: Prizes.CardCount,
                benchCount: Bench.CardCount,
                discardPileCount: DiscardPile.CardCount,
                performedOncePerTurnActions: new(PerformedOncePerTurnActions),
                turnCounter: TurnCounter,
                playerEffects: playerEffects
            );
        }
    }
}
