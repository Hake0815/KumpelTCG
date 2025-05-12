using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    internal class ResetPokemonInPlayStateGA : GameAction
    {
        public IPokemonCardLogic PokemonToReset { get; }

        public ResetPokemonInPlayStateGA(IPokemonCardLogic pokemonToReset)
        {
            PokemonToReset = pokemonToReset;
        }
    }
}
