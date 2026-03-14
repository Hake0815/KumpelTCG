using gamecore.actionsystem;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class StartTurnGA : GameAction
    {
        public IPlayerLogic NextPlayer { get; }

        [JsonConstructor]
        public StartTurnGA(IPlayerLogic nextPlayer)
        {
            NextPlayer = nextPlayer;
        }
    }
}
