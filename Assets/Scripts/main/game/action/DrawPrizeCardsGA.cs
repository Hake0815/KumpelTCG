using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class DrawPrizeCardsGA : GameAction
    {
        public Dictionary<IPlayerLogic, int> NumberOfPrizeCardsPerPlayer { get; }
        public Dictionary<string, List<ICardLogic>> DrawnCards { get; } = new();

        public DrawPrizeCardsGA(Dictionary<IPlayerLogic, int> numberOfPrizeCardsPerPlayer)
        {
            NumberOfPrizeCardsPerPlayer = numberOfPrizeCardsPerPlayer;
        }
    }
}
