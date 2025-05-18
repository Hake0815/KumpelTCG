using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class MovePokemonToBenchGA : GameAction
    {
        public IPokemonCardLogic Pokemon { get; }

        public MovePokemonToBenchGA(IPokemonCardLogic pokemon)
        {
            Pokemon = pokemon;
        }
    }
}
