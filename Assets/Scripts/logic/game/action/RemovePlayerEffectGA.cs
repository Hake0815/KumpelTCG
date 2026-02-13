using gamecore.actionsystem;
using gamecore.effect;

namespace gamecore.game.action
{
    class RemovePlayerEffectGA : GameAction
    {
        public RemovePlayerEffectGA(IPlayerLogic player, PlayerEffectAbstract effect)
        {
            Player = player;
            Effect = effect;
        }

        public IPlayerLogic Player { get; }
        public PlayerEffectAbstract Effect { get; }
    }
}
