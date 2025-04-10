using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    internal class BenchPokemonGA : GameAction
    {
        public BenchPokemonGA(ICardLogic card)
        {
            Card = card;
        }

        public ICardLogic Card { get; }
    }
}
