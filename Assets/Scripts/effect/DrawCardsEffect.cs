using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.effect
{
    public class DrawCardsEffect : IEffect
    {
        private int Amount { get; set; }

        public DrawCardsEffect(int amount)
        {
            Amount = amount;
        }

        public void Perform(ICard card)
        {
            ActionSystem.INSTANCE.Perform(new DrawCardGA(Amount, card.Owner));
        }
    }
}
