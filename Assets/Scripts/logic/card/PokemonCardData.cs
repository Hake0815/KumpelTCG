using System.Collections.Generic;

namespace gamecore.card
{
    internal interface IPokemonCardData : ICardData
    {
        Stage Stage { get; }
        string EvolvesFrom { get; }
        EnergyType Type { get; }
        EnergyType Weakness { get; }
        EnergyType Resistance { get; }
        int RetreatCost { get; }
        int MaxHP { get; }
        List<IAttackLogic> Attacks { get; }
        IAbilityLogic Ability { get; }
        int NumberOfPrizeCardsOnKnockout { get; }
    }

    class PokemonCardData : IPokemonCardData
    {
        public string Id { get; }
        public string Name { get; }
        public Stage Stage { get; }
        public string EvolvesFrom { get; }
        public EnergyType Type { get; }
        public EnergyType Weakness { get; }
        public EnergyType Resistance { get; }
        public int RetreatCost { get; }
        public int MaxHP { get; }
        public List<IAttackLogic> Attacks { get; }
        public int NumberOfPrizeCardsOnKnockout { get; }
        public IAbilityLogic Ability { get; }

        public PokemonCardData(
            string id,
            string name,
            Stage stage,
            string evolvesFrom,
            EnergyType type,
            EnergyType weakness,
            EnergyType resistance,
            int retreatCost,
            int maxHP,
            List<IAttackLogic> attacks,
            int numberOfPrizeCardsOnKnockout,
            IAbilityLogic ability = null
        )
        {
            Id = id;
            Name = name;
            Stage = stage;
            EvolvesFrom = evolvesFrom;
            Type = type;
            Weakness = weakness;
            Resistance = resistance;
            RetreatCost = retreatCost;
            MaxHP = maxHP;
            Attacks = attacks;
            NumberOfPrizeCardsOnKnockout = numberOfPrizeCardsOnKnockout;
            Ability = ability;
        }
    }

    public enum Stage
    {
        Basic,
        Stage1,
        Stage2,
    }
}
