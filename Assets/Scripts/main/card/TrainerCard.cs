using System;
using System.Collections.Generic;
using gamecore.effect;
using gamecore.game;

namespace gamecore.card
{
    internal interface ITrainerCard : ICard
    {
        List<IEffect> Effects { get; }
        List<IPlayCondition> Conditions { get; }
    }

    internal class TrainerCard : ITrainerCard, ICardLogic
    {
        public ICardData CardData { get; }
        public ITrainerCardData TrainerCardData
        {
            get => CardData as ITrainerCardData;
        }
        public IPlayerLogic Owner { get; }
        public List<IEffect> Effects { get; }
        public List<IPlayCondition> Conditions { get; }

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
            if (!(Owner.IsActive && Owner.Hand.Cards.Contains(this)))
                return false;
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
    }
}
