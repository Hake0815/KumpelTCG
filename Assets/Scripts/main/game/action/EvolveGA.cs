using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class EvolveGA : GameAction
    {
        public EvolveGA(IPokemonCardLogic targetPokemon, IPokemonCardLogic newPokemon)
        {
            TargetPokemon = targetPokemon;
            NewPokemon = newPokemon;
        }

        public IPokemonCardLogic TargetPokemon { get; }
        public IPokemonCardLogic NewPokemon { get; }
    }
}
