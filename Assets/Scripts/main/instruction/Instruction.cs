using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.instruction
{
    internal interface IInstruction
    {
        void Perform(ICardLogic card, ActionSystem actionSystem);
        InstructionJson ToSerializable();
    }
}
