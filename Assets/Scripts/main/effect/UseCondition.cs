using gamecore.card;

namespace gamecore.effect
{
    internal interface IUseCondition
    {
        bool IsMet(ICardLogic card);
    }
}
