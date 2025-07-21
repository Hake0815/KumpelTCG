using gamecore.game;
using Newtonsoft.Json;

namespace gamecore.card
{
    class BasicEnergyCard : EnergyCard
    {
        public BasicEnergyCard(IEnergyCardData energyCardData, IPlayerLogic owner, int deckId)
            : base(energyCardData, owner, deckId) { }

        [JsonConstructor]
        public BasicEnergyCard(string name, string id, int deckId, IPlayerLogic owner)
            : base(name, id, deckId, owner) { }

        public override CardSubtype CardSubtype => CardSubtype.BasicEnergy;

        public override bool IsBasicEnergyCard() => true;
    }
}
