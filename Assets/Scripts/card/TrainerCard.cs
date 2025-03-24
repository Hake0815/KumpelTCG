using System;
using System.Collections.Generic;
using gamecore.effect;
using gamecore.game;
using UnityEngine;

namespace gamecore.card
{
    public interface ITrainerCard : ICard
    {
        public List<IEffect> Effects { get; }
        public List<IPlayCondition> Conditions { get; }
    }

    public class TrainerCard : ITrainerCard
    {
        public ICardData CardData { get; }
        public ITrainerCardData TrainerCardData
        {
            get => CardData as ITrainerCardData;
        }
        public IPlayer Owner { get; }
        public List<IEffect> Effects { get; }
        public List<IPlayCondition> Conditions { get; }

        public event Action CardDiscarded;

        public TrainerCard(ITrainerCardData cardData, IPlayer owner)
        {
            CardData = cardData;
            Owner = owner;
            Effects = cardData.Effects;
            Conditions = cardData.Conditions;
        }

        public static ITrainerCard Of(ITrainerCardData cardData, IPlayer owner)
        {
            return new TrainerCard(cardData, owner);
        }

        public void Play()
        {
            if (IsPlayable())
                PerformEffects();
        }

        public bool IsPlayable()
        {
            if (!Owner.IsActive)
                return false;
            foreach (var condition in Conditions)
            {
                if (!condition.IsMet(this))
                {
                    Debug.Log("Card is not playable");
                    return false;
                }
            }
            return true;
        }

        private void PerformEffects() // During effect performing the card still is in the player's hand, might be a problem later
        {
            Debug.Log("Performing effects");
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
