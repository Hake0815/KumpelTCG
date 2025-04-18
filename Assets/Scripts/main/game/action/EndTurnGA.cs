using gamecore.actionsystem;

namespace gamecore.game.action
{
    internal class EndTurnGA : GameAction
    {
        public IPlayer NextPlayer { get; set; }
    }
}
