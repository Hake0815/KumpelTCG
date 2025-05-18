using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class RevealCardsFromDeckGA : GameAction
    {
        public IPlayerLogic Player { get; }
        public int Count { get; }
        public List<ICardLogic> RevealedCards { get; } = new();

        public RevealCardsFromDeckGA(IPlayerLogic player, int count)
        {
            Player = player;
            Count = count;
        }
    }
}
