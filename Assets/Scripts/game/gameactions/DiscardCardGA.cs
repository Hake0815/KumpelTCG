using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    public class DiscardCardsFromHandGA : GameAction
    {
        public List<ICard> Cards { get; private set; }

        public DiscardCardsFromHandGA(List<ICard> cards)
        {
            Cards = cards;
        }
    }
}
