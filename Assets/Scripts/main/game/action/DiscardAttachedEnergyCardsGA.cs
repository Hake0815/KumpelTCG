using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class DiscardAttachedEnergyCardsGA : GameAction
    {
        public DiscardAttachedEnergyCardsGA(
            IPokemonCardLogic pokemon,
            List<IEnergyCardLogic> energyCards
        )
        {
            Pokemon = pokemon;
            EnergyCards = energyCards;
        }

        public IPokemonCardLogic Pokemon { get; }
        public List<IEnergyCardLogic> EnergyCards { get; }
    }
}
