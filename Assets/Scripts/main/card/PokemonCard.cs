using System;
using System.Collections.Generic;
using System.Linq;
using gamecore.actionsystem;
using gamecore.common;
using gamecore.game;
using gamecore.game.action;

namespace gamecore.card
{
    public interface IPokemonCard : ICard
    {
        Stage Stage { get; }
        List<IAttack> Attacks { get; }
        List<IEnergyCard> AttachedEnergyCards { get; }
        int Damage { get; }
        event Action<IEnergyCard> EnergyAttached;
        event Action DamageModified;
    }

    internal interface IPokemonCardLogic : ICardLogic, IPokemonCard
    {
        PokemonType Type { get; set; }
        PokemonType Weakness { get; set; }
        PokemonType Resistance { get; set; }
        void AttachEnergy(IEnergyCardLogic energy);
        new List<IEnergyCardLogic> AttachedEnergyCards { get; }
        new List<IAttackLogic> Attacks { get; }
        List<IAttackLogic> GetUsableAttacks();
        bool IsActive();
        void TakeDamage(int damage);
    }

    internal class PokemonCard : IPokemonCardLogic
    {
        public IPokemonCardData PokemonCardData { get; }
        public ICardData CardData => PokemonCardData;
        public List<IAttackLogic> Attacks { get; } = new();
        public IPlayerLogic Owner { get; }
        public Stage Stage => PokemonCardData.Stage;

        public List<IEnergyCardLogic> AttachedEnergyCards { get; } = new();

        List<IEnergyCard> IPokemonCard.AttachedEnergyCards =>
            AttachedEnergyCards.AsEnumerable().Cast<IEnergyCard>().ToList();

        List<IAttack> IPokemonCard.Attacks => Attacks.AsEnumerable().Cast<IAttack>().ToList();
        public PokemonType Type { get; set; }
        public PokemonType Weakness { get; set; }
        public PokemonType Resistance { get; set; }

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

        public event Action CardDiscarded;
        public event Action DamageModified;
        public event Action<IEnergyCard> EnergyAttached;

        public PokemonCard(IPokemonCardData cardData, IPlayerLogic owner)
        {
            PokemonCardData = cardData;
            Owner = owner;
            Attacks = cardData.Attacks.ConvertAll(attack => attack.Clone());
            Weakness = cardData.Weakness;
            Resistance = cardData.Resistance;
            Type = cardData.Type;
        }

        public void Discard()
        {
            Owner.DiscardPile.AddCards(new() { this });
            Damage = 0;
            CardDiscarded?.Invoke();
        }

        public bool IsPlayable()
        {
            return Stage == Stage.Basic && !Owner.Bench.Full;
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

        private bool IsAttackUsable(IAttackLogic attack, List<PokemonType> availableEnergyTypes)
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
                ActionSystem.INSTANCE.Perform(new BenchPokemonGA(this));
                Damage = 0;
            }
        }

        public void PlayWithTargets(List<ICardLogic> targets)
        {
            throw new IlleagalActionException("Pokemon cards cannot be played with a target.");
        }

        public bool IsPlayableWithTargets()
        {
            return false;
        }

        public bool IsEnergyCard()
        {
            return false;
        }

        public void AttachEnergy(IEnergyCardLogic energy)
        {
            AttachedEnergyCards.Add(energy);
            EnergyAttached?.Invoke(energy);
        }

        public List<ICardLogic> GetTargets()
        {
            throw new IlleagalActionException("Pokemon cards cannot be played with a target.");
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
    }
}
