using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class DrawPrizeCardsGA : GameAction
    {
        public Dictionary<IPlayerLogic, int> NumberOfPrizeCardsPerPlayer { get; }
        public Dictionary<string, List<ICardLogic>> DrawnCards { get; } = new();

        [JsonConstructor]
        public DrawPrizeCardsGA(Dictionary<IPlayerLogic, int> numberOfPrizeCardsPerPlayer)
        {
            NumberOfPrizeCardsPerPlayer = numberOfPrizeCardsPerPlayer;
        }
    }
}
