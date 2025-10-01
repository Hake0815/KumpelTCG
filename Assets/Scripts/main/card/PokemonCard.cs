using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.effect;
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
        List<EnergyType> AttachedEnergy { get; }

        [JsonIgnore]
        List<IPokemonCard> PreEvolutions { get; }

        [JsonIgnore]
        IAbility Ability { get; }

        [JsonIgnore]
        int Damage { get; }

        [JsonIgnore]
        int MaxHP { get; }

        [JsonIgnore]
        int RetreatCost { get; }

        [JsonIgnore]
        Dictionary<Type, IPokemonEffect> PokemonEffects { get; }
        event Action<List<IEnergyCard>> OnAttachedEnergyChanged;
        event Action DamageModified;
        event Action Evolved;
    }

    internal interface IPokemonCardLogic : ICardLogic, IPokemonCard
    {
        [JsonIgnore]
        EnergyType PokemonType { get; set; }

        [JsonIgnore]
        EnergyType Weakness { get; set; }

        [JsonIgnore]
        EnergyType Resistance { get; set; }

        [JsonIgnore]
        int NumberOfPrizeCardsOnKnockout { get; set; }
        void AttachEnergyCards(List<IEnergyCardLogic> energyCards);

        [JsonIgnore]
        new List<IEnergyCardLogic> AttachedEnergyCards { get; }

        [JsonIgnore]
        new List<IAttackLogic> Attacks { get; }
        List<IAttackLogic> GetUsableAttacks();

        [JsonIgnore]
        new IAbilityLogic Ability { get; }
        bool HasUsableAbility();
        bool IsActive();
        bool IsKnockedOut();
        void TakeDamage(int damage);
        bool CanPayRetreatCost();
        void DiscardEnergy(List<IEnergyCardLogic> energyCardsToDiscard);
        void WasEvolved();
        void SetPutInPlay();
        bool HasEffect<T>()
            where T : IPokemonEffect;
        void AddEffect(IPokemonEffect effect);
        void RemoveEffect(IPokemonEffect effect);
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
        public EnergyType PokemonType { get; set; }
        public EnergyType Weakness { get; set; }
        public EnergyType Resistance { get; set; }
        public int MaxHP { get; private set; }
        public int RetreatCost { get; private set; }
        public int NumberOfPrizeCardsOnKnockout { get; set; }
        public Dictionary<Type, IPokemonEffect> PokemonEffects { get; } = new();
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
        public List<EnergyType> AttachedEnergy
        {
            get
            {
                var providedEnergy = new List<EnergyType>();
                foreach (var energy in AttachedEnergyCards)
                {
                    providedEnergy.Add(energy.ProvidedEnergyType);
                }
                return providedEnergy;
            }
        }

        public string EvolvesFrom => _pokemonCardData.EvolvesFrom;
        public List<IPokemonCard> PreEvolutions { get; } = new();

        public IAbilityLogic Ability { get; }

        IAbility IPokemonCard.Ability => Ability;

        public int DeckId { get; }

        public CardType CardType => CardType.Pokemon;

        public CardSubtype CardSubtype =>
            Stage switch
            {
                Stage.Basic => CardSubtype.BasicPokemon,
                Stage.Stage1 => CardSubtype.Stage1Pokemon,
                Stage.Stage2 => CardSubtype.Stage2Pokemon,
                _ => throw new NotImplementedException(),
            };

        public PositionKnowledge OwnerPositionKnowledge { get; set; }
        public PositionKnowledge OpponentPositionKnowledge { get; set; }
        public int TopDeckPositionIndex { get; set; }

        public event Action DamageModified;
        public event Action<List<IEnergyCard>> OnAttachedEnergyChanged;
        public event Action Evolved;

        [JsonConstructor]
        public PokemonCard(string name, string id, int deckId, IPlayerLogic owner)
        {
            Owner = owner;
            DeckId = deckId;
        }

        public PokemonCard(IPokemonCardData cardData, IPlayerLogic owner, int deckId)
        {
            _pokemonCardData = cardData;
            Owner = owner;
            Attacks = cardData.Attacks.ConvertAll(attack => attack.Clone());
            Weakness = cardData.Weakness;
            Resistance = cardData.Resistance;
            PokemonType = cardData.Type;
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

        private List<EnergyType> GetAvailableEnergyTypes()
        {
            var availableEnergyTypes = new List<EnergyType>();
            foreach (var energy in AttachedEnergyCards)
            {
                availableEnergyTypes.Add(energy.ProvidedEnergyType);
            }

            return availableEnergyTypes;
        }

        private static bool IsAttackUsable(
            IAttackLogic attack,
            List<EnergyType> availableEnergyTypes
        )
        {
            foreach (var attackCost in attack.Cost)
            {
                if (attackCost != EnergyType.Colorless)
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

        public bool IsPokemonCard() => true;

        public bool IsTrainerCard() => false;

        public bool IsSupporterCard() => false;

        public bool IsItemCard() => false;

        public bool IsEnergyCard() => false;

        public bool IsBasicEnergyCard() => false;

        public bool IsPlayable()
        {
            return Stage == Stage.Basic && !Owner.Bench.Full;
        }

        public void Play()
        {
            if (!Owner.Bench.Full)
            {
                ActionSystem.INSTANCE.AddReaction(new BenchPokemonGA(this));
                Damage = 0;
            }
            else
            {
                throw new Exception("Bench is full");
            }
        }

        public bool IsPlayableWithTargets()
        {
            if (Owner.TurnCounter < 2)
                return false;
            if (
                Owner.ActivePokemon.Name == EvolvesFrom
                && !Owner.ActivePokemon.HasEffect<PutIntoPlayThisTurnEffect>()
            )
                return true;
            foreach (var benchPokemon in Owner.Bench.Cards)
            {
                if (
                    benchPokemon.Name == EvolvesFrom
                    && !(benchPokemon as IPokemonCardLogic).HasEffect<PutIntoPlayThisTurnEffect>()
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
                    && !(benchPokemon as IPokemonCardLogic).HasEffect<PutIntoPlayThisTurnEffect>()
                )
                    targets.Add(benchPokemon);
            }
            if (
                Owner.ActivePokemon.Name == EvolvesFrom
                && !Owner.ActivePokemon.HasEffect<PutIntoPlayThisTurnEffect>()
            )
                targets.Add(Owner.ActivePokemon);

            return targets;
        }

        public void PlayWithTargets(List<ICardLogic> targets)
        {
            ActionSystem.INSTANCE.AddReaction(new EvolveGA(targets[0] as IPokemonCardLogic, this));
        }

        public void SetPutInPlay()
        {
            ((IPokemonEffect)new PutIntoPlayThisTurnEffect()).Apply(this);
        }

        public int GetNumberOfTargets() => 1;

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

        public bool HasEffect<T>()
            where T : IPokemonEffect
        {
            return PokemonEffects.ContainsKey(typeof(T));
        }

        public void AddEffect(IPokemonEffect effect)
        {
            PokemonEffects[effect.GetType()] = effect;
        }

        public void RemoveEffect(IPokemonEffect effect)
        {
            PokemonEffects.Remove(effect.GetType());
        }

        public CardJson ToSerializable()
        {
            var preEvolutionIds = new List<int>();
            foreach (var preEvolution in PreEvolutions)
            {
                preEvolutionIds.Add(preEvolution.DeckId);
            }
            var attachedEnergy = new List<int>();
            foreach (var energy in AttachedEnergyCards)
            {
                attachedEnergy.Add(energy.DeckId);
            }

            return new CardJson(
                cardData: _pokemonCardData.ToSerializable(),
                deckId: DeckId,
                currentDamage: Damage,
                attachedEnergy: attachedEnergy,
                preEvolutionIds: preEvolutionIds
            );
        }
    }
}
