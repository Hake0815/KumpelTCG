using System;
using System.Collections.Generic;
using gamecore.effect;
using gamecore.game;

namespace gamecore.card
{
    public interface ITrainerCard : ICard
    {
        public List<IEffect> Effects { get; }
        public List<IPlayCondition> Conditions { get; }
    }

    internal class TrainerCard : ITrainerCard, ICardLogic
    {
        private IPlayerLogic _owner;
        public ICardData CardData { get; }
        public ITrainerCardData TrainerCardData
        {
            get => CardData as ITrainerCardData;
        }
        IPlayerLogic ICardLogic.Owner
        {
            get => _owner;
        }
        public List<IEffect> Effects { get; }
        public List<IPlayCondition> Conditions { get; }

        public event Action CardDiscarded;

        internal TrainerCard(ITrainerCardData cardData, IPlayerLogic owner)
        {
            CardData = cardData;
            _owner = owner;
            Effects = cardData.Effects;
            Conditions = cardData.Conditions;
        }

        void ICardLogic.Play()
        {
            PerformEffects();
        }

        bool ICardLogic.IsPlayable()
        {
            if (!(_owner.IsActive && _owner.Hand.Cards.Contains(this)))
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

        void ICardLogic.Discard()
        {
            _owner.DiscardPile.AddCards(new() { this });
            CardDiscarded?.Invoke();
        }

        bool ICardLogic.IsTrainerCard()
        {
            return true;
        }

        bool ICardLogic.IsPokemonCard()
        {
            return false;
        }
    }
}
