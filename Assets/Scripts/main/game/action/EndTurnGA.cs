using gamecore.actionsystem;

namespace gamecore.game.action
{
    class EndTurnGA : GameAction
    {
        public IPlayerLogic NextPlayer { get; set; }
    }
}
