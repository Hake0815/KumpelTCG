using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.game;
using Newtonsoft.Json;

namespace gamecore.gamegame.action
{
    class CheckWinConditionGA : GameAction
    {
        public List<IPlayerLogic> Players { get; }
        public bool GameEnded { get; set; } = false;
        public IPlayerLogic Winner { get; set; }

        [JsonConstructor]
        public CheckWinConditionGA(List<IPlayerLogic> players)
        {
            Players = players;
        }
    }
}
