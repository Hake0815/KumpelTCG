using System;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.instruction
{
    class ShowSelectedCardsInstruction : IInstruction
    {
        private readonly string _selectionId;

        public ShowSelectedCardsInstruction(string selectionId)
        {
            _selectionId = selectionId;
        }

        public void Perform(ICardLogic card)
        {
            new InstructionSubscriber<SelectCardsGA>(
                action => Reaction(action, card),
                ReactionTiming.POST
            );
        }

        private bool Reaction(SelectCardsGA action, ICardLogic card)
        {
            if (action.SelectionId != _selectionId)
                return false;
            ActionSystem.INSTANCE.AddReaction(new ShowCardsGA(action.SelectedCards));
            return true;
        }

        public InstructionJson ToSerializable()
        {
            return new InstructionJson(instructionType: "show_cards");
        }
    }
}
