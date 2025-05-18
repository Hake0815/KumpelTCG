using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class ResetPokemonTurnStateGA : GameAction
    {
        public IPokemonCardLogic PokemonToReset { get; }

        public ResetPokemonTurnStateGA(IPokemonCardLogic pokemonToReset)
        {
            PokemonToReset = pokemonToReset;
        }
    }
}
