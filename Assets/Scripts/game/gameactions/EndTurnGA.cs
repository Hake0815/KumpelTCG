using gamecore.actionsystem;

namespace gamecore.game.action
{
    public class EndTurnGA : GameAction
    {
        public IPlayer NextPlayer { get; set; }
    }
}
