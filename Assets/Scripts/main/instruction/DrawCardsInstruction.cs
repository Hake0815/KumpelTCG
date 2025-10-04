using System.Collections.Generic;
using System.Threading.Tasks;
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
                instructionType: "take_cards_to_hand",
                data: new Dictionary<string, object> { { "count", Count }, { "from", "deck" } }
            );
        }
    }
}
