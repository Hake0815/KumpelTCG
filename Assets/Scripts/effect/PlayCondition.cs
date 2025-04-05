using gamecore.card;

namespace gamecore.effect
{
    internal interface IPlayCondition
    {
        bool IsMet(ICardLogic card);
    }
}
