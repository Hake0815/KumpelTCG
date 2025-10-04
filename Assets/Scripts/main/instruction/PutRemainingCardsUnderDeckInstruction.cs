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
        public void Perform(ICardLogic card, ActionSystem actionSystem)
        {
            new InstructionSubscriber<DoOnSelectionGA>(
                action => Reaction(action, card, actionSystem),
                ReactionTiming.POST,
                actionSystem
            );
        }

        private static bool Reaction(
            DoOnSelectionGA action,
            ICardLogic card,
            ActionSystem actionSystem
        )
        {
            actionSystem.AddReaction(
                new PutRemainingCardsUnderDeckGA(card.Owner, action.RemainingCards)
            );
            return true;
        }

        public InstructionJson ToSerializable()
        {
            return new InstructionJson(
                instructionType: "put_cards_in_deck",
                data: new() { { "return_type", "under" }, { "from", "selection_remainder" } }
            );
        }
    }
}
