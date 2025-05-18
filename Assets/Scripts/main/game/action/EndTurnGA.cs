using gamecore.actionsystem;

namespace gamecore.game.action
{
    class EndTurnGA : GameAction
    {
        public IPlayer NextPlayer { get; set; }
    }
}
