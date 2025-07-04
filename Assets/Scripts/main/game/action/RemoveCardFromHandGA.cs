using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class RemoveCardFromHandGA : GameAction
    {
        public ICardLogic card;

        public RemoveCardFromHandGA(ICardLogic card)
        {
            this.card = card;
        }
    }
}
