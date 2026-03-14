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

        public ProtoBufInstruction ToSerializable()
        {
            return new ProtoBufInstruction
            {
                InstructionType = ProtoBufInstructionType.InstructionTypeShuffleDeck,
                Data =
                {
                    new ProtoBufInstructionData
                    {
                        InstructionDataType =
                            ProtoBufInstructionDataType.InstructionDataTypePlayerTargetData,
                        PlayerTargetData = new ProtoBufPlayerTargetInstructionData
                        {
                            PlayerTarget = ProtoBufPlayerTarget.PlayerTargetSelf,
                        },
                    },
                },
            };
        }
    }
}
