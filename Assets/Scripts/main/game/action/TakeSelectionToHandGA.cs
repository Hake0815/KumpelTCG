using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class TakeSelectionToHandGA : GameAction
    {
        [JsonConstructor]
        public TakeSelectionToHandGA(List<ICardLogic> options, IPlayerLogic player, int amount)
        {
            Options = options;
            Player = player;
            Amount = amount;
        }

        public List<ICardLogic> Options { get; }
        public IPlayerLogic Player { get; }
        public int Amount { get; }
        public List<ICardLogic> RemainingCards { get; } = new();
    }
}
