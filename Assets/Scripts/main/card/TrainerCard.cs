using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gamecore.common;
using gamecore.effect;
using gamecore.game;

namespace gamecore.card
{
    public interface ITrainerCard : ICard { }

    internal interface ITrainerCardLogic : ITrainerCard, ICardLogic
    {
        List<IEffect> Effects { get; }
        List<IUseCondition> Conditions { get; }
    }

    class TrainerCard : ITrainerCardLogic
    {
        public ICardData CardData { get; }
        public ITrainerCardData TrainerCardData
        {
            get => CardData as ITrainerCardData;
        }
        public IPlayerLogic Owner { get; }
        public List<IEffect> Effects { get; }
        public List<IUseCondition> Conditions { get; }

        IPlayer ICard.Owner => Owner;

        public event Action CardDiscarded;

        public TrainerCard(ITrainerCardData cardData, IPlayerLogic owner)
        {
            CardData = cardData;
            Owner = owner;
            Effects = cardData.Effects;
            Conditions = cardData.Conditions;
        }

        public void Play()
        {
            PerformEffects();
        }

        public bool IsPlayable()
        {
            foreach (var condition in Conditions)
            {
                if (!condition.IsMet(this))
                {
                    return false;
                }
            }
            return true;
        }

        private void PerformEffects() // During effect performing the card is still in the player's hand, might be a problem later
        {
            foreach (var effect in Effects)
            {
                effect.Perform(this);
            }
        }

        public void Discard()
        {
            Owner.DiscardPile.AddCards(new() { this });
            CardDiscarded?.Invoke();
        }

        public bool IsTrainerCard()
        {
            return true;
        }

        public bool IsPokemonCard()
        {
            return false;
        }

        public void PlayWithTargets(List<ICardLogic> targets)
        {
            throw new IlleagalActionException("Trainer cards cannot be played with a target, yet.");
        }

        public bool IsPlayableWithTargets()
        {
            return false;
        }

        public bool IsEnergyCard()
        {
            return false;
        }

        public List<ICardLogic> GetPossibleTargets()
        {
            throw new IlleagalActionException("Trainer cards cannot be played with a target, yet.");
        }

        public int GetNumberOfTargets()
        {
            throw new IlleagalActionException("Trainer cards cannot be played with a target, yet.");
        }
    }
}
