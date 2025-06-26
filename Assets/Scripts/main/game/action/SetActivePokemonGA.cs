using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class SetActivePokemonGA : GameAction
    {
        public IPokemonCardLogic Card { get; }

        [JsonConstructor]
        public SetActivePokemonGA(IPokemonCardLogic card)
        {
            Card = card;
        }
    }
}
