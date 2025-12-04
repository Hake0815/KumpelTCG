using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using gamecore.serialization;

namespace gamecore.instruction
{
    class ShuffleDeckInstruction : IInstruction
    {
        public void Perform(ICardLogic card, ActionSystem actionSystem)
        {
            actionSystem.AddReaction(new ShuffleDeckGA(card.Owner));
        }

        public InstructionJson ToSerializable()
        {
            return new InstructionJson(
                instructionType: InstructionType.ShuffleDeck,
                data: new()
                {
                    new InstructionDataJson(
                        InstructionDataType.PlayerTargetData,
                        new PlayerTargetInstructionDataJson(PlayerTarget.Self)
                    ),
                }
            );
        }
    }
}
