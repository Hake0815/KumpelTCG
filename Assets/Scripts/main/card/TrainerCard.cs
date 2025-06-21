using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gamecore.common;
using gamecore.effect;
using gamecore.game;
using Newtonsoft.Json;

namespace gamecore.card
{
    public interface ITrainerCard : ICard { }

    internal interface ITrainerCardLogic : ITrainerCard, ICardLogic
    {
        [JsonIgnore]
        List<IEffect> Effects { get; }

        [JsonIgnore]
        List<IUseCondition> Conditions { get; }
    }

    class TrainerCard : ITrainerCardLogic
    {
        private readonly ITrainerCardData _trainerCardData;
        public string Name => _trainerCardData.Name;
        public string Id => _trainerCardData.Id;
        public IPlayerLogic Owner { get; }
        public List<IEffect> Effects { get; }
        public List<IUseCondition> Conditions { get; }

        IPlayer ICard.Owner => Owner;

        public int DeckId { get; }

        public event Action CardDiscarded;

        public TrainerCard(ITrainerCardData cardData, IPlayerLogic owner, int deckId)
        {
            _trainerCardData = cardData;
            Owner = owner;
            Effects = cardData.Effects;
            Conditions = cardData.Conditions;
            DeckId = deckId;
        }

        [JsonConstructor]
        public TrainerCard(string name, string id, int deckId, IPlayerLogic owner)
        {
            DeckId = deckId;
            Owner = owner;
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
