using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using gamecore.serialization;

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

        public ProtoBufInstruction ToSerializable()
        {
            return new ProtoBufInstruction
            {
                InstructionType = ProtoBufInstructionType.InstructionTypePutInDeck,
                Data =
                {
                    new ProtoBufInstructionData
                    {
                        InstructionDataType =
                            ProtoBufInstructionDataType.InstructionDataTypeReturnToDeckTypeData,
                        ReturnToDeckTypeData = new ProtoBufReturnToDeckTypeInstructionData
                        {
                            ReturnToDeckType = ProtoBufReturnToDeckType.ReturnToDeckTypeUnder,
                            FromPosition = ProtoBufCardPosition.CardPositionSelectedCardsRemainder,
                        },
                    },
                },
            };
        }
    }
}
