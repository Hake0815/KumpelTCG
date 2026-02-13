using gamecore.actionsystem;
using gamecore.card;
using gamecore.serialization;

namespace gamecore.instruction
{
    internal interface IInstruction
    {
        void Perform(ICardLogic card, ActionSystem actionSystem);
        ProtoBufInstruction ToSerializable();
    }
}
