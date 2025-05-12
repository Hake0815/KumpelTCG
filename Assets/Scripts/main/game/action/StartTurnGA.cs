using gamecore.actionsystem;

namespace gamecore.game.action
{
    internal class StartTurnGA : GameAction
    {
        public IPlayerLogic NextPlayer { get; }

        public StartTurnGA(IPlayerLogic nextPlayer)
        {
            NextPlayer = nextPlayer;
        }
    }
}
