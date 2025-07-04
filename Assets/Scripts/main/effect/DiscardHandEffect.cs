using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.effect
{
    class DiscardHandEffect : IEffect
    {
        public void Perform(ICardLogic card)
        {
            var handCards = card.Owner.Hand.Cards;
            ActionSystem.INSTANCE.AddReaction(
                new RemoveCardsFromHandGA(new(handCards), card.Owner)
            );
            ActionSystem.INSTANCE.AddReaction(new DiscardCardsGA(new(handCards)));
        }
    }
}
