using System.Collections.Generic;
using gamecore.actionsystem;

namespace gamecore.game.action
{
    class PromoteGA : GameAction
    {
        public PromoteGA(List<IPlayerLogic> players)
        {
            Players = players;
        }

        public List<IPlayerLogic> Players { get; }
    }
}
