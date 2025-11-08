using System;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using gamecore.serialization;

namespace gamecore.instruction
{
    class ShowSelectedCardsInstruction : IInstruction
    {
        private readonly string _selectionId;

        public ShowSelectedCardsInstruction(string selectionId)
        {
            _selectionId = selectionId;
        }

        public void Perform(ICardLogic card, ActionSystem actionSystem)
        {
            new InstructionSubscriber<SelectCardsGA>(
                action => Reaction(action, actionSystem),
                ReactionTiming.POST,
                actionSystem
            );
        }

        private bool Reaction(SelectCardsGA action, ActionSystem actionSystem)
        {
            if (action.SelectionId != _selectionId)
                return false;
            actionSystem.AddReaction(new ShowCardsGA(action.SelectedCards));
            return true;
        }

        public InstructionJson ToSerializable()
        {
            return new InstructionJson(instructionType: InstructionType.ShowCards);
        }
    }
}
