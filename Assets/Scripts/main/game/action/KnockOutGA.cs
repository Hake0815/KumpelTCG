using System;
using System.Deployment.Internal;
using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class KnockOutGA : GameAction
    {
        public IPokemonCardLogic Pokemon { get; }
        public int NumberOfPrizeCards { get; set; }

        public KnockOutGA(IPokemonCardLogic pokemon)
        {
            Pokemon = pokemon;
        }
    }
}
