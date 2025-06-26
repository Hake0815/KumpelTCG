using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class BenchPokemonGA : GameAction
    {
        [JsonConstructor]
        public BenchPokemonGA(IPokemonCardLogic card)
        {
            Card = card;
        }

        public IPokemonCardLogic Card { get; }
    }
}
