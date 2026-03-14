using gamecore.card;
using gamecore.serialization;

namespace gamecore.instruction.filter
{
    abstract class FilterNode
    {
        public abstract bool Matches(ICardLogic card, ICardLogic sourceCard);
        public abstract ProtoBufFilter ToSerializable();
    }
}
