using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class KnockOutCheckGA : GameAction
    {
        public List<IPokemonCardLogic> KnockedOutPokemon { get; } = new();
        public List<IPlayerLogic> Players { get; }

        [JsonConstructor]
        public KnockOutCheckGA(List<IPlayerLogic> players)
        {
            Players = players;
        }
    }
}
