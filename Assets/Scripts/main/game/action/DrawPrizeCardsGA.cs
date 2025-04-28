using System.Collections.Generic;
using gamecore.actionsystem;

namespace gamecore.game.action
{
    internal class DrawPrizeCardsGA : GameAction
    {
        public Dictionary<IPlayerLogic, int> NumberOfPrizeCardsPerPlayer { get; }

        public DrawPrizeCardsGA(Dictionary<IPlayerLogic, int> numberOfPrizeCardsPerPlayer)
        {
            NumberOfPrizeCardsPerPlayer = numberOfPrizeCardsPerPlayer;
        }
    }
}
