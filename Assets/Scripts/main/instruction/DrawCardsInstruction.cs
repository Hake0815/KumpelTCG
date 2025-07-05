using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.instruction
{
    class DrawCardsInstruction : IInstruction
    {
        public int Amount { get; }

        public DrawCardsInstruction(int amount)
        {
            Amount = amount;
        }

        public void Perform(ICardLogic card)
        {
            ActionSystem.INSTANCE.AddReaction(new DrawCardGA(Amount, card.Owner));
        }
    }
}
