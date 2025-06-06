using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.common;
using gamecore.game;
using gamecore.game.action;
using Newtonsoft.Json;

namespace gamecore.card
{
    public interface IEnergyCard : ICard { }

    internal interface IEnergyCardLogic : IEnergyCard, ICardLogic
    {
        [JsonIgnore]
        PokemonType ProvidedEnergyType { get; }
    }

    class EnergyCard : IEnergyCardLogic
    {
        public static string ATTACHED_ENERGY_FOR_TURN = "attachedEnergyForTurn";

        public EnergyCard(IEnergyCardData energyCardData, IPlayerLogic owner, int deckId)
        {
            _energyCardData = energyCardData;
            Owner = owner;
            DeckId = deckId;
        }

        private readonly IEnergyCardData _energyCardData;
        public string Name => _energyCardData.Name;
        public string Id => _energyCardData.Id;
        public IPlayerLogic Owner { get; }

        public int DeckId { get; }

        public PokemonType ProvidedEnergyType => _energyCardData.Type;

        public event Action CardDiscarded;

        public void Discard()
        {
            Owner.DiscardPile.AddCards(new() { this });
            CardDiscarded?.Invoke();
        }

        public List<ICardLogic> GetPossibleTargets()
        {
            var targets = new List<ICardLogic>();
            targets.AddRange(Owner.Bench.Cards);
            targets.Add(Owner.ActivePokemon);
            return targets;
        }

        public int GetNumberOfTargets() => 1;

        public bool IsEnergyCard()
        {
            return true;
        }

        public bool IsPlayable()
        {
            return false;
        }

        public bool IsPlayableWithTargets()
        {
            return !Owner.PerformedOncePerTurnActions.Contains(ATTACHED_ENERGY_FOR_TURN);
        }

        public bool IsPokemonCard()
        {
            return false;
        }

        public bool IsTrainerCard()
        {
            return false;
        }

        public void Play()
        {
            throw new IlleagalActionException("Energy cards can only be played with a target");
        }

        public void PlayWithTargets(List<ICardLogic> targets)
        {
            ActionSystem.INSTANCE.AddReaction(
                new AttachEnergyFromHandForTurnGA(this, targets[0] as IPokemonCardLogic)
            );
        }
    }
}
