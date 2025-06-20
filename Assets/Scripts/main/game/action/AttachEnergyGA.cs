using System;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class AttachEnergyGA : GameAction
    {
        public IEnergyCardLogic EnergyCard { get; }
        public IPokemonCardLogic TargetPokemon { get; }

        [JsonConstructor]
        public AttachEnergyGA(IEnergyCardLogic energyCard, IPokemonCardLogic targetPokemon)
        {
            EnergyCard = energyCard;
            TargetPokemon = targetPokemon;
        }
    }
}
