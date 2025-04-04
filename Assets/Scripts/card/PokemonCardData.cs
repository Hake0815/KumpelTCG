using System.Collections.Generic;

namespace gamecore.card
{
    public interface IPokemonCardData : ICardData
    {
        public Stage Stage { get; }
        public PokemonType Type { get; }
        public PokemonType Weakness { get; }
        public PokemonType Resistance { get; }
        public int RetreatCost { get; }
        public int MaxHP { get; }
        public List<IAttack> Attacks { get; }
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
        public List<IAttack> Attacks { get; }

        public PokemonCardData(
            string id,
            string name,
            Stage stage,
            PokemonType type,
            PokemonType weakness,
            PokemonType resistance,
            int retreatCost,
            int maxHP,
            List<IAttack> attacks
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

    public enum PokemonType
    {
        None,
        Grass,
        Fire,
        Water,
        Lightning,
        Fighting,
        Psychic,
        Colorless,
        Darrkness,
        Metal,
        Dragon,
    }
}
