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

        public InstructionJson ToSerializable()
        {
            return new InstructionJson(
                instructionType: InstructionType.TakeToHand,
                data: new()
                {
                    new InstructionDataJson(
                        InstructionDataType.CardAmountData,
                        new CardAmountInstructionDataJson(
                            new IntRange(Count, Count),
                            CardPosition.Deck
                        )
                    ),
                }
            );
        }
    }
}
