using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.effect
{
    class SelectCardsFromHandEffect : IEffect
    {
        public int Amount { get; }

        public SelectCardsFromHandEffect(int amount)
        {
            Amount = amount;
        }

        public void Perform(ICardLogic card)
        {
            ActionSystem.INSTANCE.AddReaction(
                new SelectCardsGA(
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
