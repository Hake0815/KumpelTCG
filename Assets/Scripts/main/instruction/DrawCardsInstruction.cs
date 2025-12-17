using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using gamecore.instruction.filter;
using gamecore.serialization;

namespace gamecore.instruction
{
    class DrawCardsInstruction : IInstruction
    {
        public int Count { get; }

        public DrawCardsInstruction(int count)
        {
            Count = count;
        }

        public void Perform(ICardLogic card, ActionSystem actionSystem)
        {
            actionSystem.AddReaction(new DrawCardGA(Count, card.Owner));
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
                            Amount = new ProtoBufIntRange { Min = Count, Max = Count },
                            FromPosition = ProtoBufCardPosition.CardPositionDeck,
                        },
                    },
                },
            };
        }
    }
}
