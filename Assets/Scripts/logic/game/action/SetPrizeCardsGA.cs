using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class SetPrizeCardsGA : GameAction
    {
        public Dictionary<string, List<ICardLogic>> PrizeCards { get; set; }
    }
}
