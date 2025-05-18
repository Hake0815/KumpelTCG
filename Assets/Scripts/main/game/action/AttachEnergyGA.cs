using System;
using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class AttachEnergyGA : GameAction
    {
        public IEnergyCardLogic EnergyCard { get; }
        public IPokemonCardLogic TargetPokemon { get; }

        public AttachEnergyGA(IEnergyCardLogic energyCard, IPokemonCardLogic targetPokemon)
        {
            EnergyCard = energyCard;
            TargetPokemon = targetPokemon;
        }
    }
}
