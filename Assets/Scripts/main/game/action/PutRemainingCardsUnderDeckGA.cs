using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class PutRemainingCardsUnderDeckGA : GameAction
    {
        public IPlayerLogic Player { get; }
        public List<ICardLogic> RemainingCards { get; }

        public PutRemainingCardsUnderDeckGA(IPlayerLogic player, List<ICardLogic> remainingCards)
        {
            Player = player;
            RemainingCards = remainingCards;
        }
    }
}
