using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class RetreatGA : GameAction
    {
        public IPokemonCardLogic Pokemon { get; }
        public List<IEnergyCardLogic> EnergyCardsToDiscard { get; }

        public RetreatGA(IPokemonCardLogic pokemon, List<IEnergyCardLogic> energyCardsToDiscard)
        {
            Pokemon = pokemon;
            EnergyCardsToDiscard = energyCardsToDiscard;
        }
    }
}
