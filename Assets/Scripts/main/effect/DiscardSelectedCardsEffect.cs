using System;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.effect
{
    class DiscardSelectedCardsEffect : IEffect
    {
        public void Perform(ICardLogic card)
        {
            new EffectSubscriber<SelectCardsGA>(action => Reaction(action), ReactionTiming.POST);
        }

        private SelectCardsGA Reaction(SelectCardsGA action)
        {
            ActionSystem.INSTANCE.AddReaction(new DiscardCardsGA(action.SelectedCards));
            return action;
        }
    }
}
