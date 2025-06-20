using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class ResetPokemonTurnStateGA : GameAction
    {
        public IPokemonCardLogic PokemonToReset { get; }

        [JsonConstructor]
        public ResetPokemonTurnStateGA(IPokemonCardLogic pokemonToReset)
        {
            PokemonToReset = pokemonToReset;
        }
    }
}
