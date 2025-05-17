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

        public async Task Perform(ICardLogic card)
        {
            await ActionSystem.INSTANCE.Perform(new DrawCardGA(Amount, card.Owner));
        }
    }
}
