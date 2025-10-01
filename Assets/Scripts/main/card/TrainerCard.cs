using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.common;
using gamecore.game;
using gamecore.game.action;
using gamecore.instruction;
using Newtonsoft.Json;

namespace gamecore.card
{
    public interface ITrainerCard : ICard { }

    internal interface ITrainerCardLogic : ITrainerCard, ICardLogic
    {
        [JsonIgnore]
        List<IInstruction> Instructions { get; }

        [JsonIgnore]
        List<IUseCondition> Conditions { get; }
    }

    abstract class TrainerCard : ITrainerCardLogic
    {
        private readonly ITrainerCardData _trainerCardData;
        public string Name => _trainerCardData.Name;
        public string Id => _trainerCardData.Id;
        public IPlayerLogic Owner { get; }
        public List<IInstruction> Instructions { get; }
        public List<IUseCondition> Conditions { get; }
        IPlayer ICard.Owner => Owner;

        public int DeckId { get; }

        public CardType CardType => CardType.Trainer;

        public abstract CardSubtype CardSubtype { get; }
        public PositionKnowledge OwnerPositionKnowledge { get; set; }
        public PositionKnowledge OpponentPositionKnowledge { get; set; }
        public int TopDeckPositionIndex { get; set; }

        protected TrainerCard(ITrainerCardData cardData, IPlayerLogic owner, int deckId)
        {
            _trainerCardData = cardData;
            Owner = owner;
            Instructions = cardData.Instructions;
            Conditions = cardData.Conditions;
            DeckId = deckId;
        }

        [JsonConstructor]
        protected TrainerCard(string name, string id, int deckId, IPlayerLogic owner)
        {
            DeckId = deckId;
            Owner = owner;
        }

        public virtual void Play()
        {
            ActionSystem.INSTANCE.AddReaction(new RemoveCardsFromHandGA(new() { this }, Owner));
            PerformInstructions();
        }

        public virtual bool IsPlayable()
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

        private void PerformInstructions()
        {
            foreach (var instruction in Instructions)
            {
                instruction.Perform(this);
            }
        }

        public void Discard()
        {
            Owner.DiscardPile.AddCards(new() { this });
        }

        public bool IsTrainerCard() => true;

        public abstract bool IsSupporterCard();
        public abstract bool IsItemCard();

        public bool IsPokemonCard() => false;

        public bool IsEnergyCard() => false;

        public bool IsBasicEnergyCard() => false;

        public void PlayWithTargets(List<ICardLogic> targets)
        {
            throw new IlleagalActionException("Trainer cards cannot be played with a target, yet.");
        }

        public bool IsPlayableWithTargets() => false;

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
