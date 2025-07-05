using System;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using UnityEngine;

namespace gamecore.instruction
{
    class PutRemainingCardsUnderDeckInstruction : IInstruction
    {
        public void Perform(ICardLogic card)
        {
            new InstructionSubscriber<DoOnSelectionGA>(
                action => Reaction(action, card),
                ReactionTiming.POST
            );
        }

        private static bool Reaction(DoOnSelectionGA action, ICardLogic card)
        {
            ActionSystem.INSTANCE.AddReaction(
                new PutRemainingCardsUnderDeckGA(card.Owner, action.RemainingCards)
            );
            return true;
        }
    }
}
