using gamecore.game;
using Newtonsoft.Json;

namespace gamecore.card
{
    class ItemCard : TrainerCard
    {
        public override CardSubtype CardSubtype => CardSubtype.Item;

        public ItemCard(ITrainerCardData cardData, IPlayerLogic owner, int deckId)
            : base(cardData, owner, deckId) { }

        [JsonConstructor]
        public ItemCard(string name, string id, int deckId, IPlayerLogic owner)
            : base(name, id, deckId, owner) { }

        public override bool IsSupporterCard() => false;

        public override bool IsItemCard() => true;
    }
}
