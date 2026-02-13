using gamecore.actionsystem;

namespace gamecore.game.action
{
    class ClearPlayerTurnTraitsGA : GameAction
    {
        public ClearPlayerTurnTraitsGA(IPlayerLogic player)
        {
            Player = player;
        }

        public IPlayerLogic Player { get; }
    }
}
