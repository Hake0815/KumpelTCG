using gamecore.card;

namespace gamecore.instruction.filter
{
    abstract class FilterNode
    {
        public abstract bool Matches(ICardLogic card, ICardLogic sourceCard);
        public abstract object ToSerializable();
    }
}
