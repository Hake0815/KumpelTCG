using gamecore.card;
using gamecore.instruction.filter;
using gamecore.serialization;

namespace gamecore.instruction
{
    class HasCardsInDeck : IUseCondition
    {
        public bool IsMet(ICardLogic card) => card.Owner.Deck.CardCount > 0;

        public ConditionJson ToSerializable()
        {
            return new ConditionJson(
                conditionType: ConditionType.HasCards,
                new() { new CardAmountInstructionDataJson(new IntRange(1, 60), CardPosition.Deck) }
            );
        }
    }
}
