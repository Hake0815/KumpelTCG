using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class RemoveCardsFromHandGA : GameAction
    {
        public List<ICardLogic> Cards { get; }
        public IPlayerLogic Player { get; }

        public RemoveCardsFromHandGA(List<ICardLogic> cards, IPlayerLogic player)
        {
            Cards = new(cards);
            Player = player;
        }
    }
}
