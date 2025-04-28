using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    internal class KnockOutCheckGA : GameAction
    {
        public List<IPokemonCardLogic> KnockedOutPokemon { get; } = new();
        public List<IPlayerLogic> Players { get; } = new();

        public KnockOutCheckGA(List<IPlayerLogic> players)
        {
            Players = players;
        }
    }
}
