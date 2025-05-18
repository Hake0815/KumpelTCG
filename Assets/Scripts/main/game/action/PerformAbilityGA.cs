using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class PerformAbilityGA : GameAction
    {
        public IPokemonCardLogic Pokemon;

        public PerformAbilityGA(IPokemonCardLogic pokemon)
        {
            Pokemon = pokemon;
        }
    }
}
