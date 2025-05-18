namespace gamecore.card
{
    internal interface IEnergyCardData : ICardData
    {
        public PokemonType Type { get; }
        public EnergyCardType EnergyCardType { get; }
    }

    class EnergyCardData : IEnergyCardData
    {
        public EnergyCardData(
            string id,
            string name,
            PokemonType type,
            EnergyCardType energyCardType
        )
        {
            Type = type;
            EnergyCardType = energyCardType;
            Name = name;
            Id = id;
        }

        public PokemonType Type { get; }
        public EnergyCardType EnergyCardType { get; }

        public string Name { get; }

        public string Id { get; }
    }

    public enum EnergyCardType
    {
        Basic,
        Special,
    }
}
