using System;
using System.Collections.Generic;
using gamecore.effect;
using gamecore.game;
using UnityEngine;

namespace gamecore.card
{
    public interface ICard
    {
        public CardData CardData { get; set; }
        public IPlayer Owner { get; }
        public List<IEffect> Effects { get; }
        public List<IPlayCondition> Conditions { get; }
        public event Action CardDiscarded;
        public bool IsPlayable();

        public string Name
        {
            get => CardData.Name;
        }

        void Discard();
        void PerformEffects();
    }

    public class CardDummy : ICard
    {
        public CardData CardData { get; set; }
        public IPlayer Owner { get; private set; }
        public List<IEffect> Effects { get; }
        public List<IPlayCondition> Conditions { get; }
        public event Action CardDiscarded;

        public CardDummy(CardData cardData, IPlayer owner)
        {
            CardData = cardData;
            Owner = owner;
            Effects = CardData.Effects;
            Conditions = CardData.Conditions;
        }

        public void Discard()
        {
            Owner.DiscardPile.AddCards(new() { this });
            CardDiscarded?.Invoke();
        }

        public void PerformEffects()
        {
            foreach (var effect in Effects)
            {
                Debug.Log("Performing effect: " + effect.GetType().Name);
                effect.Perform(this);
            }
        }

        public static ICard Of(CardData cardData, IPlayer owner)
        {
            return new CardDummy(cardData, owner);
        }

        public bool IsPlayable()
        {
            if (!Owner.IsActive)
            {
                return false;
            }
            foreach (var condition in Conditions)
            {
                if (!condition.IsMet(this))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
