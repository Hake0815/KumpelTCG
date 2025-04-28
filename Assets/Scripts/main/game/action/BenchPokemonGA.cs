using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    internal class BenchPokemonGA : GameAction
    {
        public BenchPokemonGA(IPokemonCardLogic card)
        {
            Card = card;
        }

        public IPokemonCardLogic Card { get; }
    }
}
