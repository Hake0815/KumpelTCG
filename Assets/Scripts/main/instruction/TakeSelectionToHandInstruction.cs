using System;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using UnityEngine;

namespace gamecore.instruction
{
    class TakeSelectionToHandInstruction : IInstruction
    {
        public void Perform(ICardLogic card)
        {
            new InstructionSubscriber<SelectCardsGA>(
                action => Reaction(action, card),
                ReactionTiming.POST
            );
        }

        private static bool Reaction(SelectCardsGA action, ICardLogic card)
        {
            if (action.WasReactedTo)
                return false;
            ActionSystem.INSTANCE.AddReaction(
                new TakeSelectionToHandGA(action.SelectedCards, card.Owner, action.RemainingCards)
            );
            action.WasReactedTo = true;
            return true;
        }

        public InstructionJson ToSerializable()
        {
            return new InstructionJson(
                instructionType: "take_cards_to_hand",
                data: new() { { "count", "all" }, { "from", "selection" } }
            );
        }
    }
}
