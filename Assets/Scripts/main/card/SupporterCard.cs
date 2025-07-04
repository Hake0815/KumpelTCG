using gamecore.game;
using Newtonsoft.Json;

namespace gamecore.card
{
    class SupporterCard : TrainerCard
    {
        public SupporterCard(ITrainerCardData cardData, IPlayerLogic owner, int deckId)
            : base(cardData, owner, deckId) { }

        [JsonConstructor]
        public SupporterCard(string name, string id, int deckId, IPlayerLogic owner)
            : base(name, id, deckId, owner) { }

        public override bool IsSupporterCard() => true;

        public override bool IsItemCard() => false;
    }
}
