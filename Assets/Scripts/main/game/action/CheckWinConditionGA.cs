using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.game;

namespace gamecore.gamegame.action
{
    class CheckWinConditionGA : GameAction
    {
        public List<IPlayerLogic> Players { get; }

        public CheckWinConditionGA(List<IPlayerLogic> players)
        {
            Players = players;
        }
    }
}
