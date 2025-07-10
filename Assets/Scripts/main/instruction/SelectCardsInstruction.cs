using gamecore.card;
using gamecore.instruction.filter;

namespace gamecore.instruction
{
    abstract class SelectCardsInstruction : IInstruction
    {
        protected SelectCardsInstruction(IntRange countRange, FilterNode filter)
        {
            CountRange = countRange;
            Filter = filter;
        }

        public IntRange CountRange { get; }
        public FilterNode Filter { get; }

        public abstract void Perform(ICardLogic card);
    }
}
