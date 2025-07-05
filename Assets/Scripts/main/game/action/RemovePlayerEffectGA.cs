using gamecore.actionsystem;
using gamecore.effect;

namespace gamecore.game.action
{
    class RemovePlayerEffectGA : GameAction
    {
        public RemovePlayerEffectGA(IPlayerLogic player, IPlayerEffect effect)
        {
            Player = player;
            Effect = effect;
        }

        public IPlayerLogic Player { get; }
        public IPlayerEffect Effect { get; }
    }
}
