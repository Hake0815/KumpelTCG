using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.game;
using gamecore.game.action;
using Newtonsoft.Json;
using UnityEngine;

namespace gamecore.card
{
    public interface IPokemonCard : ICard
    {
        [JsonIgnore]
        Stage Stage { get; }

        [JsonIgnore]
        string EvolvesFrom { get; }

        [JsonIgnore]
        List<IAttack> Attacks { get; }

        [JsonIgnore]
        List<IEnergyCard> AttachedEnergyCards { get; }

        [JsonIgnore]
        List<PokemonType> AttachedEnergy { get; }

        [JsonIgnore]
        List<IPokemonCard> PreEvolutions { get; }

        [JsonIgnore]
        IAbility Ability { get; }

        [JsonIgnore]
        bool AbilityUsedThisTurn { get; }

        [JsonIgnore]
        int Damage { get; }

        [JsonIgnore]
        int MaxHP { get; }

        [JsonIgnore]
        int RetreatCost { get; }
        event Action<List<IEnergyCard>> OnAttachedEnergyChanged;
        event Action DamageModified;
        event Action Evolved;
    }

    internal interface IPokemonCardLogic : ICardLogic, IPokemonCard, IActionSubscriber<EndTurnGA>
    {
        [JsonIgnore]
        PokemonType Type { get; set; }

        [JsonIgnore]
        PokemonType Weakness { get; set; }

        [JsonIgnore]
        PokemonType Resistance { get; set; }

        [JsonIgnore]
        int NumberOfPrizeCardsOnKnockout { get; set; }
        void AttachEnergyCards(List<IEnergyCardLogic> energyCards);

        [JsonIgnore]
        new List<IEnergyCardLogic> AttachedEnergyCards { get; }

        [JsonIgnore]
        new List<IAttackLogic> Attacks { get; }
        List<IAttackLogic> GetUsableAttacks();

        [JsonIgnore]
        bool PutIntoPlayThisTurn { get; set; }

        [JsonIgnore]
        new IAbilityLogic Ability { get; }
        bool HasUsableAbility();

        [JsonIgnore]
        new bool AbilityUsedThisTurn { get; set; }
        bool IsActive();
        bool IsKnockedOut();
        void TakeDamage(int damage);
        bool CanPayRetreatCost();
        void DiscardEnergy(List<IEnergyCardLogic> energyCardsToDiscard);
        void WasEvolved();
        void SetPutInPlay();
    }

    class PokemonCard : IPokemonCardLogic
    {
        public static string RETREATED = "retreated";

        private IPokemonCardData _pokemonCardData { get; }
        public string Name => _pokemonCardData.Name;
        public string Id => _pokemonCardData.Id;
        public List<IAttackLogic> Attacks { get; }
        public IPlayerLogic Owner { get; }
        public Stage Stage => _pokemonCardData.Stage;

        public List<IEnergyCardLogic> AttachedEnergyCards { get; } = new();

        List<IEnergyCard> IPokemonCard.AttachedEnergyCards =>
            AttachedEnergyCards.Cast<IEnergyCard>().ToList();

        List<IAttack> IPokemonCard.Attacks => Attacks.Cast<IAttack>().ToList();
        public PokemonType Type { get; set; }
        public PokemonType Weakness { get; set; }
        public PokemonType Resistance { get; set; }
        public int MaxHP { get; private set; }
        public int RetreatCost { get; private set; }
        public int NumberOfPrizeCardsOnKnockout { get; set; }
        public bool AbilityUsedThisTurn { get; set; } = false;

        private int _damage = 0;
        public int Damage
        {
            get => _damage;
            private set
            {
                _damage = value;
                DamageModified?.Invoke();
            }
        }
        public List<PokemonType> AttachedEnergy
        {
            get
            {
                var providedEnergy = new List<PokemonType>();
                foreach (var energy in AttachedEnergyCards)
                {
                    providedEnergy.Add(energy.ProvidedEnergyType);
                }
                return providedEnergy;
            }
        }

        public string EvolvesFrom => _pokemonCardData.EvolvesFrom;
        public List<IPokemonCard> PreEvolutions { get; } = new();
        public bool PutIntoPlayThisTurn { get; set; }

        public IAbilityLogic Ability { get; }

        IAbility IPokemonCard.Ability => Ability;

        public int DeckId { get; }
        public event Action CardDiscarded;
        public event Action DamageModified;
        public event Action<List<IEnergyCard>> OnAttachedEnergyChanged;
        public event Action Evolved;

        public PokemonCard(IPokemonCardData cardData, IPlayerLogic owner, int deckId)
        {
            _pokemonCardData = cardData;
            Owner = owner;
            Attacks = cardData.Attacks.ConvertAll(attack => attack.Clone());
            Weakness = cardData.Weakness;
            Resistance = cardData.Resistance;
            Type = cardData.Type;
            MaxHP = cardData.MaxHP;
            NumberOfPrizeCardsOnKnockout = cardData.NumberOfPrizeCardsOnKnockout;
            RetreatCost = cardData.RetreatCost;
            Ability = cardData.Ability;
            DeckId = deckId;
        }

        public void Discard()
        {
            Owner.DiscardPile.AddCards(new() { this });
            Damage = 0;
            CardDiscarded?.Invoke();
        }

        public List<IAttackLogic> GetUsableAttacks()
        {
            var usableAttacks = new List<IAttackLogic>();
            foreach (var attack in Attacks)
            {
                var availableEnergyTypes = GetAvailableEnergyTypes();
                if (IsAttackUsable(attack, availableEnergyTypes))
                    usableAttacks.Add(attack);
            }
            return usableAttacks;
        }

        private List<PokemonType> GetAvailableEnergyTypes()
        {
            var availableEnergyTypes = new List<PokemonType>();
            foreach (var energy in AttachedEnergyCards)
            {
                availableEnergyTypes.Add(energy.ProvidedEnergyType);
            }

            return availableEnergyTypes;
        }

        private static bool IsAttackUsable(
            IAttackLogic attack,
            List<PokemonType> availableEnergyTypes
        )
        {
            foreach (var attackCost in attack.Cost)
            {
                if (attackCost != PokemonType.Colorless)
                {
                    if (!availableEnergyTypes.Remove(attackCost))
                        return false;
                }
                else
                {
                    if (availableEnergyTypes.Count <= 0)
                        return false;
                    availableEnergyTypes.RemoveAt(0);
                }
            }
            return true;
        }

        public bool IsPokemonCard()
        {
            return true;
        }

        public bool IsTrainerCard()
        {
            return false;
        }

        public bool IsPlayable()
        {
            return Stage == Stage.Basic && !Owner.Bench.Full;
        }

        public void Play()
        {
            if (Owner.ActivePokemon == null)
            {
                Owner.ActivePokemon = this;
                Owner.Hand.RemoveCard(this);
                Damage = 0;
                return;
            }
            if (!Owner.Bench.Full)
            {
                ActionSystem.INSTANCE.AddReaction(new BenchPokemonGA(this));
                Damage = 0;
            }
        }

        public bool IsPlayableWithTargets()
        {
            if (Owner.TurnCounter < 2)
                return false;
            if (Owner.ActivePokemon.Name == EvolvesFrom && !Owner.ActivePokemon.PutIntoPlayThisTurn)
                return true;
            foreach (var benchPokemon in Owner.Bench.Cards)
            {
                if (
                    benchPokemon.Name == EvolvesFrom
                    && !(benchPokemon as IPokemonCardLogic).PutIntoPlayThisTurn
                )
                    return true;
            }
            return false;
        }

        public List<ICardLogic> GetPossibleTargets()
        {
            var targets = new List<ICardLogic>();
            foreach (var benchPokemon in Owner.Bench.Cards)
            {
                if (
                    benchPokemon.Name == EvolvesFrom
                    && !(benchPokemon as IPokemonCardLogic).PutIntoPlayThisTurn
                )
                    targets.Add(benchPokemon);
            }
            if (Owner.ActivePokemon.Name == EvolvesFrom && !Owner.ActivePokemon.PutIntoPlayThisTurn)
                targets.Add(Owner.ActivePokemon);

            return targets;
        }

        public void PlayWithTargets(List<ICardLogic> targets)
        {
            ActionSystem.INSTANCE.AddReaction(new EvolveGA(targets[0] as IPokemonCardLogic, this));
        }

        public void SetPutInPlay()
        {
            PutIntoPlayThisTurn = true;
            ActionSystem.INSTANCE.SubscribeToGameAction<EndTurnGA>(this, ReactionTiming.PRE);
        }

        public int GetNumberOfTargets() => 1;

        public bool IsEnergyCard()
        {
            return false;
        }

        public void AttachEnergyCards(List<IEnergyCardLogic> energyCards)
        {
            AttachedEnergyCards.AddRange(energyCards);
            OnAttachedEnergyChanged?.Invoke(energyCards.Cast<IEnergyCard>().ToList());
        }

        public bool IsActive()
        {
            return Owner.ActivePokemon == this;
        }

        public void TakeDamage(int damage)
        {
            Damage += damage;
            DamageModified?.Invoke();
        }

        public bool IsKnockedOut()
        {
            return Damage >= MaxHP;
        }

        public bool CanPayRetreatCost()
        {
            return AttachedEnergy.Count >= RetreatCost;
        }

        public void DiscardEnergy(List<IEnergyCardLogic> energyCardsToDiscard)
        {
            foreach (var energyCard in energyCardsToDiscard)
            {
                energyCard.Discard();
                AttachedEnergyCards.Remove(energyCard);
            }
            OnAttachedEnergyChanged?.Invoke(new());
        }

        public void WasEvolved()
        {
            Evolved?.Invoke();
        }

        public EndTurnGA React(EndTurnGA action)
        {
            ActionSystem.INSTANCE.AddReaction(new ResetPokemonTurnStateGA(this));
            return action;
        }

        public bool HasUsableAbility()
        {
            if (Ability == null)
                return false;
            foreach (var condition in Ability.Conditions)
            {
                if (!condition.IsMet(this))
                    return false;
            }
            return true;
        }
    }
}
