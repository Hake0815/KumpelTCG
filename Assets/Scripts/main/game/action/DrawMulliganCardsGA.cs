using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class DrawMulliganCardsGA : GameAction
    {
        public int Amount { get; }
        public IPlayerLogic Player { get; }
        public List<ICardLogic> DrawnCards { get; } = new();

        [JsonConstructor]
        public DrawMulliganCardsGA(int amount, IPlayerLogic player)
        {
            Amount = amount;
            Player = player;
        }
    }
}
