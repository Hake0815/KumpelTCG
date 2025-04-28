using System.Collections.Generic;

namespace gamecore.card
{
    internal interface IPokemonCardData : ICardData
    {
        Stage Stage { get; }
        PokemonType Type { get; }
        PokemonType Weakness { get; }
        PokemonType Resistance { get; }
        int RetreatCost { get; }
        int MaxHP { get; }
        List<IAttackLogic> Attacks { get; }
    }

    internal class PokemonCardData : IPokemonCardData
    {
        public string Id { get; }
        public string Name { get; }
        public Stage Stage { get; }
        public PokemonType Type { get; }
        public PokemonType Weakness { get; }
        public PokemonType Resistance { get; }
        public int RetreatCost { get; }
        public int MaxHP { get; }
        public List<IAttackLogic> Attacks { get; }

        public PokemonCardData(
            string id,
            string name,
            Stage stage,
            PokemonType type,
            PokemonType weakness,
            PokemonType resistance,
            int retreatCost,
            int maxHP,
            List<IAttackLogic> attacks
        )
        {
            Id = id;
            Name = name;
            Stage = stage;
            Type = type;
            Weakness = weakness;
            Resistance = resistance;
            RetreatCost = retreatCost;
            MaxHP = maxHP;
            Attacks = attacks;
        }
    }

    public enum Stage
    {
        Basic,
        Stage1,
        Stage2,
    }
}
