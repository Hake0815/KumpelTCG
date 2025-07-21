using gamecore.card;

namespace gamecore.instruction
{
    class HasCardsInDeck : IUseCondition
    {
        public bool IsMet(ICardLogic card) => card.Owner.Deck.CardCount > 0;

        public ConditionJson ToSerializable()
        {
            return new ConditionJson(
                conditionType: "has_cards",
                new() { { "count", 1 }, { "in", "deck" } }
            );
        }
    }
}
