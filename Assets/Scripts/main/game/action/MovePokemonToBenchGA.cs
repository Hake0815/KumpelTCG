using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    internal class MovePokemonToBenchGA : GameAction
    {
        public IPokemonCardLogic Pokemon { get; }

        public MovePokemonToBenchGA(IPokemonCardLogic pokemon)
        {
            Pokemon = pokemon;
        }
    }
}
