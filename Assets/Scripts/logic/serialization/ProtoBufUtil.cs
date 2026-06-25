namespace gamecore.serialization
{
    public class ProtoBufUtil
    {
        public static ProtoBufCardStatic CreateUnknownCard(int deckId)
        {
            return new ProtoBufCardStatic
            {
                Name = "Unknown",
                CardType = ProtoBufCardType.CardTypeUnknown,
                CardSubtype = ProtoBufCardSubtype.CardSubtypeUnknown,
                DeckId = deckId,
            };
        }
    }
}
