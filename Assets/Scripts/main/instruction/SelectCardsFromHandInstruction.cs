using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.instruction
{
    class SelectCardsFromHandInstruction : IInstruction
    {
        public int Amount { get; }

        public SelectCardsFromHandInstruction(int amount)
        {
            Amount = amount;
        }

        public void Perform(ICardLogic card)
        {
            ActionSystem.INSTANCE.AddReaction(
                new SelectExactCardsGA(
                    card.Owner,
                    Amount,
                    card.Owner.Hand,
                    c => c != card,
                    SelectCardsGA.SelectedCardsOrigin.Hand
                )
            );
        }
    }
}
