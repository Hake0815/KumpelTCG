using gamecore.actionsystem;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class ShuffleDeckGA : GameAction
    {
        public IPlayerLogic Player { get; }

        [JsonConstructor]
        public ShuffleDeckGA(IPlayerLogic player)
        {
            Player = player;
        }
    }
}
