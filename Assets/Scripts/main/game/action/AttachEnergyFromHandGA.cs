using System;
using gamecore.card;

namespace gamecore.game.action
{
    class AttachEnergyFromHandGA : AttachEnergyGA
    {
        public AttachEnergyFromHandGA(IEnergyCardLogic energyCard, IPokemonCardLogic targetPokemon)
            : base(energyCard, targetPokemon) { }
    }
}
