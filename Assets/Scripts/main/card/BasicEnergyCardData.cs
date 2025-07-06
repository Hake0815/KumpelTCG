namespace gamecore.card
{
    class BasicEnergyCardData : EnergyCardData
    {
        public BasicEnergyCardData(
            string id,
            string name,
            PokemonType type,
            EnergyCardType energyCardType
        )
            : base(id, name, type, energyCardType) { }
    }
}
