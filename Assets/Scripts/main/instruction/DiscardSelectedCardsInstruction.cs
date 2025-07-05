using System;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.instruction
{
    class DiscardSelectedCardsInstruction : IInstruction
    {
        public void Perform(ICardLogic card)
        {
            new InstructionSubscriber<SelectCardsGA>(
                action => Reaction(action),
                ReactionTiming.POST
            );
        }

        private static bool Reaction(SelectCardsGA action)
        {
            if (action.WasReactedTo)
                return false;

            ActionSystem.INSTANCE.AddReaction(new DiscardCardsGA(action.SelectedCards));
            action.WasReactedTo = true;
            return true;
        }
    }
}
