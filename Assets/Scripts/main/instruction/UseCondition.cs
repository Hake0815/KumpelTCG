using gamecore.card;
using gamecore.serialization;

namespace gamecore.instruction
{
    internal interface IUseCondition
    {
        bool IsMet(ICardLogic card);
        ConditionJson ToSerializable();
    }
}
