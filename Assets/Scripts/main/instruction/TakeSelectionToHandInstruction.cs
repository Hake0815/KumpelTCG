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

        public ProtoBufInstruction ToSerializable()
        {
            return new ProtoBufInstruction
            {
                InstructionType = ProtoBufInstructionType.InstructionTypeTakeToHand,
                Data =
                {
                    new ProtoBufInstructionData
                    {
                        InstructionDataType =
                            ProtoBufInstructionDataType.InstructionDataTypeCardAmountData,
                        CardAmountData = new ProtoBufCardAmountInstructionData
                        {
                            Amount = new ProtoBufIntRange { Min = -1, Max = -1 },
                            FromPosition = ProtoBufCardPosition.CardPositionSelectedCards,
                        },
                    },
                },
            };
        }
    }
}
