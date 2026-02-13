using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class DrawCardGA : GameAction
    {
        public int Amount { get; }
        public IPlayerLogic Player { get; }
        public List<ICardLogic> DrawnCards { get; } = new();

        [JsonConstructor]
        public DrawCardGA(int amount, IPlayerLogic player)
        {
            Amount = amount;
            Player = player;
        }
    }
}
