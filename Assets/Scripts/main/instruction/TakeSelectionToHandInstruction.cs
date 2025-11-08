using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using gamecore.instruction.filter;
using gamecore.serialization;

namespace gamecore.instruction
{
    class TakeSelectionToHandInstruction : IInstruction
    {
        private readonly string _selectionId;

        public TakeSelectionToHandInstruction(string selectionId)
        {
            _selectionId = selectionId;
        }

        public void Perform(ICardLogic card, ActionSystem actionSystem)
        {
            new InstructionSubscriber<SelectCardsGA>(
                action => Reaction(action, card, actionSystem),
                ReactionTiming.POST,
                actionSystem
            );
        }

        private bool Reaction(SelectCardsGA action, ICardLogic card, ActionSystem actionSystem)
        {
            if (action.SelectionId != _selectionId)
                return false;
            actionSystem.AddReaction(
                new TakeSelectionToHandGA(action.SelectedCards, card.Owner, action.RemainingCards)
            );
            return true;
        }

        public InstructionJson ToSerializable()
        {
            return new InstructionJson(
                instructionType: InstructionType.TakeToHand,
                data: new()
                {
                    new CardAmountInstructionDataJson(
                        new IntRange(-1, -1),
                        CardPosition.SelectedCards
                    ),
                }
            );
        }
    }
}
