using gamecore.card;

namespace gamecore.instruction
{
    internal interface IUseCondition
    {
        bool IsMet(ICardLogic card);
    }
}
