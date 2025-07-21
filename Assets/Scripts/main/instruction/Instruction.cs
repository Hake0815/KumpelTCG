using gamecore.card;

namespace gamecore.instruction
{
    internal interface IInstruction
    {
        void Perform(ICardLogic card);
        InstructionJson ToSerializable();
    }
}
