using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class DiscardAttachedEnergyCardsGA : GameAction
    {
        [JsonConstructor]
        public DiscardAttachedEnergyCardsGA(
            IPokemonCardLogic pokemon,
            List<IEnergyCardLogic> energyCards
        )
        {
            Pokemon = pokemon;
            EnergyCards = new(energyCards);
        }

        public IPokemonCardLogic Pokemon { get; }
        public List<IEnergyCardLogic> EnergyCards { get; }
    }
}
