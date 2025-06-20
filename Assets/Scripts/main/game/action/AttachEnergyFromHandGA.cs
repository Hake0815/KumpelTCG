using System;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class AttachEnergyFromHandGA : AttachEnergyGA
    {
        [JsonConstructor]
        public AttachEnergyFromHandGA(IEnergyCardLogic energyCard, IPokemonCardLogic targetPokemon)
            : base(energyCard, targetPokemon) { }
    }
}
