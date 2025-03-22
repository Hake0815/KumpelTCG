using gamecore.card;

namespace gamecore.effect
{
    public interface IEffect
    {
        void Perform(ICard card);
    }
}
