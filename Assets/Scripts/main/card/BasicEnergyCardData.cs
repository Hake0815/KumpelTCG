namespace gamecore.card
{
    class BasicEnergyCardData : EnergyCardData
    {
        public BasicEnergyCardData(
            string id,
            string name,
            EnergyType type,
            EnergyCardType energyCardType
        )
            : base(id, name, new() { type }, energyCardType) { }
    }
}
