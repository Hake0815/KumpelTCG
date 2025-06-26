using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class EvolveGA : GameAction
    {
        [JsonConstructor]
        public EvolveGA(IPokemonCardLogic targetPokemon, IPokemonCardLogic newPokemon)
        {
            TargetPokemon = targetPokemon;
            NewPokemon = newPokemon;
        }

        public IPokemonCardLogic TargetPokemon { get; }
        public IPokemonCardLogic NewPokemon { get; }
    }
}
