namespace gamecore.serialization
{
    public class ProtoBufUtil
    {
        public static ProtoBufCard CreateUnknownCard(int deckId)
        {
            return new ProtoBufCard
            {
                Name = "Unknown",
                CardType = ProtoBufCardType.CardTypeUnknown,
                CardSubtype = ProtoBufCardSubtype.CardSubtypeUnknown,
                EnergyType = ProtoBufEnergyType.EnergyTypeNone,
                DeckId = deckId,
            };
        }
    }
}
