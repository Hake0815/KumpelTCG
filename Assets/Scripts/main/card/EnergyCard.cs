using System;
using System.Collections.Generic;
using System.Linq;
using gamecore.actionsystem;
using gamecore.common;
using gamecore.game;
using gamecore.game.action;
using gamecore.game.interaction;
using gamecore.serialization;
using Newtonsoft.Json;

namespace gamecore.card
{
    public interface IEnergyCard : ICard { }

    internal interface IEnergyCardLogic : IEnergyCard, ICardLogic
    {
        [JsonIgnore]
        List<EnergyType> ProvidedEnergy { get; }
    }

    abstract class EnergyCard : IEnergyCardLogic
    {
        protected EnergyCard(string name, string id, int deckId, IPlayerLogic owner)
        {
            DeckId = deckId;
            Owner = owner;
        }

        protected EnergyCard(IEnergyCardData energyCardData, IPlayerLogic owner, int deckId)
        {
            _energyCardData = energyCardData;
            Owner = owner;
            DeckId = deckId;
            ProvidedEnergy = new(energyCardData.Types);
        }

        private readonly IEnergyCardData _energyCardData;
        public string Name => _energyCardData.Name;
        public string Id => _energyCardData.Id;
        public IPlayerLogic Owner { get; }

        public int DeckId { get; }

        public List<EnergyType> ProvidedEnergy { get; }

        public CardType CardType => CardType.Energy;

        public abstract CardSubtype CardSubtype { get; }
        public PositionKnowledge OwnerPositionKnowledge { get; set; }
        public PositionKnowledge OpponentPositionKnowledge { get; set; }
        public int TopDeckPositionIndex { get; set; }

        public void Discard()
        {
            Owner.DiscardPile.AddCards(new() { this });
        }

        public List<ICardLogic> GetPossibleTargets()
        {
            var targets = new List<ICardLogic>();
            targets.AddRange(Owner.Bench.Cards);
            targets.Add(Owner.ActivePokemon);
            return targets;
        }

        public int GetNumberOfTargets() => 1;

        public bool IsPlayable() => false;

        public bool IsPlayableWithTargets()
        {
            return !Owner.PerformedOncePerTurnActions.Contains(
                OncePerTurnActionType.AttachedEnergyForTurn
            );
        }

        public bool IsPokemonCard() => false;

        public bool IsTrainerCard() => false;

        public bool IsSupporterCard() => false;

        public bool IsItemCard() => false;

        public bool IsEnergyCard() => true;

        public abstract bool IsBasicEnergyCard();

        public void Play(ActionSystem actionSystem)
        {
            throw new IllegalActionException("Energy cards can only be played with a target");
        }

        public void PlayWithTargets(List<ICardLogic> targets, ActionSystem actionSystem)
        {
            actionSystem.AddReaction(
                new AttachEnergyFromHandForTurnGA(this, targets[0] as IPokemonCardLogic)
            );
        }

        public ActionOnSelection GetTargetAction() => ActionOnSelection.AttachTo;

        public virtual ProtoBufCard ToSerializable()
        {
            var protoBufCard = new ProtoBufCard
            {
                Name = Name,
                CardType = CardType.Energy.ToProtoBuf(),
                CardSubtype = CardSubtype.ToProtoBuf(),
                DeckId = DeckId,
            };
            protoBufCard.ProvidedEnergy.Capacity = ProvidedEnergy.Count;
            foreach (var energyType in ProvidedEnergy)
            {
                protoBufCard.ProvidedEnergy.Add(energyType.ToProtoBuf());
            }
            return protoBufCard;
        }

        public virtual ProtoBufCard ToSerializable(IPokemonCard pokemonCard)
        {
            var protoBufCard = new ProtoBufCard
            {
                Name = Name,
                CardType = CardType.Energy.ToProtoBuf(),
                CardSubtype = CardSubtype.ToProtoBuf(),
                DeckId = DeckId,
                AttachedTo = pokemonCard.DeckId,
            };
            protoBufCard.ProvidedEnergy.Capacity = ProvidedEnergy.Count;
            foreach (var energyType in ProvidedEnergy)
            {
                protoBufCard.ProvidedEnergy.Add(energyType.ToProtoBuf());
            }
            return protoBufCard;
        }
    }
}
