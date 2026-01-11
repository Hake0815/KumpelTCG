using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using gamecore.instruction.filter;
using gamecore.serialization;

namespace gamecore.instruction
{
    class RevealCardsFromDeckInstruction : IInstruction
    {
        public int Count { get; }

        public RevealCardsFromDeckInstruction(int cardsToReveal)
        {
            Count = cardsToReveal;
        }

        public void Perform(ICardLogic card, ActionSystem actionSystem)
        {
            actionSystem.AddReaction(new RevealCardsFromDeckGA(card.Owner, Count));
        }

        public ProtoBufInstruction ToSerializable()
        {
            return new ProtoBufInstruction
            {
                InstructionType = ProtoBufInstructionType.InstructionTypeRevealCards,
                Data =
                {
                    new ProtoBufInstructionData
                    {
                        InstructionDataType =
                            ProtoBufInstructionDataType.InstructionDataTypeCardAmountData,
                        CardAmountData = new ProtoBufCardAmountInstructionData
                        {
                            Amount = new ProtoBufIntRange { Min = Count, Max = Count },
                            FromPosition = ProtoBufCardPosition.CardPositionDeck,
                        },
                    },
                },
            };
        }
    }
}
