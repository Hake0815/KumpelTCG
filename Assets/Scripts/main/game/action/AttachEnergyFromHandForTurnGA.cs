using System;
using gamecore.card;

namespace gamecore.game.action
{
    class AttachEnergyFromHandForTurnGA : AttachEnergyFromHandGA
    {
        public AttachEnergyFromHandForTurnGA(
            IEnergyCardLogic energyCard,
            IPokemonCardLogic targetPokemon
        )
            : base(energyCard, targetPokemon) { }
    }
}
