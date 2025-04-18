using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.common;
using gamecore.game;
using gamecore.game.action;

namespace gamecore.card
{
    public interface IPokemonCard : ICard
    {
        Stage Stage { get; }
        List<IEnergyCard> AttachedEnergies { get; }
        event Action<IEnergyCard> EnergyAttached;
    }

    internal interface IPokemonCardLogic : ICardLogic, IPokemonCard
    {
        void AttachEnergy(IEnergyCard energy);
    }

    internal class PokemonCard : IPokemonCardLogic
    {
        public ICardData CardData => PokemonCardData;

        public IPokemonCardData PokemonCardData { get; }
        public IPlayerLogic Owner { get; }
        public Stage Stage => PokemonCardData.Stage;

        public List<IEnergyCard> AttachedEnergies { get; } = new();

        public event Action CardDiscarded;
        public event Action<IEnergyCard> EnergyAttached;

        public PokemonCard(IPokemonCardData cardData, IPlayerLogic owner)
        {
            PokemonCardData = cardData;
            Owner = owner;
        }

        public void Discard()
        {
            Owner.DiscardPile.AddCards(new() { this });
            CardDiscarded?.Invoke();
        }

        public bool IsPlayable()
        {
            return Stage == Stage.Basic && !Owner.Bench.Full;
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
                return;
            }
            if (!Owner.Bench.Full)
            {
                ActionSystem.INSTANCE.Perform(new BenchPokemonGA(this));
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

        public void AttachEnergy(IEnergyCard energy)
        {
            AttachedEnergies.Add(energy);
            EnergyAttached?.Invoke(energy);
        }

        public List<ICardLogic> GetTargets()
        {
            throw new IlleagalActionException("Pokemon cards cannot be played with a target.");
        }
    }
}
