using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class BenchPokemonGA : GameAction
    {
        public BenchPokemonGA(IPokemonCardLogic card)
        {
            Card = card;
        }

        public IPokemonCardLogic Card { get; }
    }
}
