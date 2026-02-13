using System.Collections.Generic;

namespace gamecore.card
{
    internal interface IEnergyCardData : ICardData
    {
        public List<EnergyType> Types { get; }
        public EnergyCardType EnergyCardType { get; }
    }

    abstract class EnergyCardData : IEnergyCardData
    {
        protected EnergyCardData(
            string id,
            string name,
            List<EnergyType> types,
            EnergyCardType energyCardType
        )
        {
            Types = types;
            EnergyCardType = energyCardType;
            Name = name;
            Id = id;
        }

        public List<EnergyType> Types { get; }
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
