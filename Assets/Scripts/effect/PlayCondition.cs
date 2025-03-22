using gamecore.card;

namespace gamecore.effect
{
    public interface IPlayCondition
    {
        bool IsMet(ICard card);
    }
}
