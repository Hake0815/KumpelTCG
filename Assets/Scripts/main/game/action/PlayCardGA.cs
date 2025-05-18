using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class PlayCardGA : GameAction
    {
        public ICardLogic Card { get; }
        public List<ICardLogic> Targets { get; }

        public PlayCardGA(ICardLogic card, List<ICardLogic> targets = null)
        {
            Card = card;
            Targets = targets;
        }
    }
}
