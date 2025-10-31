using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.common;
using gamecore.game;
using gamecore.game.action;
using gamecore.game.interaction;
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
        public static string ATTACHED_ENERGY_FOR_TURN = "attachedEnergyForTurn";

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
            return !Owner.PerformedOncePerTurnActions.Contains(ATTACHED_ENERGY_FOR_TURN);
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

        public virtual CardJson ToSerializable()
        {
            return new CardJson(
                name: Name,
                cardType: CardType.Energy,
                cardSubtype: CardSubtype,
                providedEnergy: ProvidedEnergy,
                deckId: DeckId
            );
        }

        public virtual CardJson ToSerializable(IPokemonCard pokemonCard)
        {
            return new CardJson(
                name: Name,
                cardType: CardType.Energy,
                cardSubtype: CardSubtype,
                providedEnergy: ProvidedEnergy,
                deckId: DeckId,
                attachedTo: pokemonCard.DeckId
            );
        }
    }
}
