using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.common;
using gamecore.game;
using gamecore.game.action;
using gamecore.game.interaction;
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

        public virtual void Play(ActionSystem actionSystem)
        {
            actionSystem.AddReaction(new RemoveCardsFromHandGA(new() { this }, Owner));
            actionSystem.AddReaction(new SetCardCurrentlyPlayedGA(this));
            PerformInstructions(actionSystem);
            actionSystem.AddReaction(new UnsetCardCurrentlyPlayedGA(Owner));
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

        private void PerformInstructions(ActionSystem actionSystem)
        {
            foreach (var instruction in Instructions)
            {
                instruction.Perform(this, actionSystem);
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

        public void PlayWithTargets(List<ICardLogic> targets, ActionSystem actionSystem)
        {
            throw new IllegalActionException("Trainer cards cannot be played with a target, yet.");
        }

        public bool IsPlayableWithTargets() => false;

        public List<ICardLogic> GetPossibleTargets()
        {
            throw new IllegalActionException("Trainer cards cannot be played with a target, yet.");
        }

        public ActionOnSelection GetTargetAction()
        {
            throw new IllegalActionException("Trainer cards cannot be played with a target, yet.");
        }

        public int GetNumberOfTargets()
        {
            throw new IllegalActionException("Trainer cards cannot be played with a target, yet.");
        }

        public virtual CardJson ToSerializable()
        {
            var instructionJsons = new List<InstructionJson>();
            foreach (var instruction in Instructions)
            {
                instructionJsons.Add(instruction.ToSerializable());
            }

            var conditionJsons = new List<ConditionJson>();
            foreach (var condition in Conditions)
            {
                conditionJsons.Add(condition.ToSerializable());
            }

            return new CardJson(
                name: Name,
                cardType: CardType.Trainer,
                cardSubtype: CardSubtype,
                conditions: conditionJsons,
                instructions: instructionJsons,
                deckId: DeckId
            );
        }

        public virtual CardJson ToSerializable(IPokemonCard pokemonCard)
        {
            throw new IllegalActionException("Trainer cards cannot be attached to pokemon, yet.");
        }
    }
}
