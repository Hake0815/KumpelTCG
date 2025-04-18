using System;
using gamecore.card;

namespace gamecore.game.action
{
    internal class AttachEnergyFromHandForTurnGA : AttachEnergyFromHandGA
    {
        public AttachEnergyFromHandForTurnGA(
            IEnergyCardLogic energyCard,
            IPokemonCardLogic targetPokemon
        )
            : base(energyCard, targetPokemon) { }
    }
}
