using System;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class AttachEnergyFromHandForTurnGA : AttachEnergyFromHandGA
    {
        [JsonConstructor]
        public AttachEnergyFromHandForTurnGA(
            IEnergyCardLogic energyCard,
            IPokemonCardLogic targetPokemon
        )
            : base(energyCard, targetPokemon) { }
    }
}
