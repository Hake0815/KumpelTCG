using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class PlaySupporterGA : GameAction
    {
        public IPlayerLogic Player { get; }

        public PlaySupporterGA(IPlayerLogic player)
        {
            Player = player;
        }
    }
}
