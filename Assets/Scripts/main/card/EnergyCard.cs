using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.common;
using gamecore.game;
using gamecore.game.action;

namespace gamecore.card
{
    public interface IEnergyCard : ICard { }

    internal interface IEnergyCardLogic : IEnergyCard, ICardLogic
    {
        new IEnergyCardData CardData { get; }
        ICardData ICardLogic.CardData => CardData;
        PokemonType ProvidedEnergyType { get; }
    }

    internal class EnergyCard : IEnergyCardLogic
    {
        public static string ATTACHED_ENERGY_FOR_TURN = "attachedEnergyForTurn";

        public EnergyCard(IEnergyCardData energyCardData, IPlayerLogic owner)
        {
            EnergyCardData = energyCardData;
            Owner = owner;
        }

        public IEnergyCardData EnergyCardData { get; }
        public IPlayerLogic Owner { get; }
        public IEnergyCardData CardData => EnergyCardData;

        public PokemonType ProvidedEnergyType => EnergyCardData.Type;

        public event Action CardDiscarded;

        public void Discard()
        {
            Owner.DiscardPile.AddCards(new() { this });
            CardDiscarded?.Invoke();
        }

        public List<ICardLogic> GetTargets()
        {
            var targets = new List<ICardLogic>();
            targets.AddRange(Owner.Bench.Cards);
            targets.Add(Owner.ActivePokemon);
            return targets;
        }

        public bool IsEnergyCard()
        {
            return true;
        }

        public bool IsPlayable()
        {
            return false;
        }

        public bool IsPlayableWithTargets()
        {
            return !Owner.PerformedOncePerTurnActions.Contains(ATTACHED_ENERGY_FOR_TURN);
        }

        public bool IsPokemonCard()
        {
            return false;
        }

        public bool IsTrainerCard()
        {
            return false;
        }

        public void Play()
        {
            throw new IlleagalActionException("Energy cards can only be played with a target");
        }

        public void PlayWithTargets(List<ICardLogic> targets)
        {
            ActionSystem.INSTANCE.Perform(
                new AttachEnergyFromHandForTurnGA(this, targets[0] as IPokemonCardLogic)
            );
        }
    }
}
