using gamecore.card;

namespace gamecore.effect
{
    public interface IEffect
    {
        public void Perform(ICard card);
    }
}
