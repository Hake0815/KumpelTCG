using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class MovePokemonToBenchGA : GameAction
    {
        public IPokemonCardLogic Pokemon { get; }

        [JsonConstructor]
        public MovePokemonToBenchGA(IPokemonCardLogic pokemon)
        {
            Pokemon = pokemon;
        }
    }
}
