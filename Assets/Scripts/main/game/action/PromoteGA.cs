using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class PromoteGA : GameAction
    {
        [JsonConstructor]
        public PromoteGA(List<IPlayerLogic> players)
        {
            Players = players;
        }

        public List<IPlayerLogic> Players { get; }
        public Dictionary<string, IPokemonCardLogic> PromotedPokemon { get; } = new();
    }
}
