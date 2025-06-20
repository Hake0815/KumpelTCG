using System;
using System.Deployment.Internal;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class KnockOutGA : GameAction
    {
        public IPokemonCardLogic Pokemon { get; }
        public int NumberOfPrizeCards { get; set; }

        [JsonConstructor]
        public KnockOutGA(IPokemonCardLogic pokemon)
        {
            Pokemon = pokemon;
        }
    }
}
