using gamecore.card;
using gamecore.instruction.filter;

namespace gamecore.instruction
{
    abstract class SelectCardsInstruction : IInstruction
    {
        protected SelectCardsInstruction(IntRange countRange, FilterNode filter, string selectionId)
        {
            CountRange = countRange;
            Filter = filter;
            SelectionId = selectionId;
        }

        public IntRange CountRange { get; }
        public FilterNode Filter { get; }
        public string SelectionId { get; }

        public abstract void Perform(ICardLogic card);

        public abstract InstructionJson ToSerializable();
    }
}
