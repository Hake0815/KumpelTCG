using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

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
                data: new Dictionary<string, object> { { "count", Count }, { "from", "deck" } }
            );
        }
    }
}
