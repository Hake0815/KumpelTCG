using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class UnsetCardCurrentlyPlayedGA : GameAction
    {
        public IPlayerLogic Player { get; }

        [JsonConstructor]
        public UnsetCardCurrentlyPlayedGA(IPlayerLogic player)
        {
            Player = player;
        }
    }
}
