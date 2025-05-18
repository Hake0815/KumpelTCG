using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.effect
{
    class DrawCardsEffect : IEffect
    {
        public int Amount { get; }

        public DrawCardsEffect(int amount)
        {
            Amount = amount;
        }

        public void Perform(ICardLogic card)
        {
            ActionSystem.INSTANCE.AddReaction(new DrawCardGA(Amount, card.Owner));
        }
    }
}
