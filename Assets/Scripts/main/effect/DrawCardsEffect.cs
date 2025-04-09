using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.effect
{
    internal class DrawCardsEffect : IEffect
    {
        public int Amount { get; }

        public DrawCardsEffect(int amount)
        {
            Amount = amount;
        }

        public void Perform(ICardLogic card)
        {
            ActionSystem.INSTANCE.Perform(new DrawCardGA(Amount, card.Owner));
        }
    }
}
