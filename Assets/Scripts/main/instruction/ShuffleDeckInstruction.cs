using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.instruction
{
    class ShuffleDeckInstruction : IInstruction
    {
        public void Perform(ICardLogic card)
        {
            ActionSystem.INSTANCE.AddReaction(new ShuffleDeckGA(card.Owner));
        }

        public InstructionJson ToSerializable()
        {
            return new InstructionJson(
                instructionType: "shuffle_deck",
                data: new() { { "player", "self" } }
            );
        }
    }
}
