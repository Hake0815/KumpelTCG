using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class DiscardCardsFromHandGA : GameAction
    {
        public List<ICardLogic> Cards { get; private set; }

        public DiscardCardsFromHandGA(List<ICardLogic> cards)
        {
            Cards = cards;
        }
    }
}
