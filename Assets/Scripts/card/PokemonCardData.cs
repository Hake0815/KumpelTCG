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
    }

    public class PokemonCardData : IPokemonCardData
    {
        public string Id { get; }
        public string Name { get; }
        public Stage Stage { get; }
        public PokemonType Type { get; }
        public PokemonType Weakness { get; }
        public PokemonType Resistance { get; }
        public int RetreatCost { get; }
        public int MaxHP { get; }
    }

    public enum Stage
    {
        BASIC,
        STAGE1,
        STAGE2,
    }

    public enum PokemonType
    {
        GRASS,
        FIRE,
        WATER,
        LIGHTNIG,
        FIGHTING,
        PSYCHIC,
        COLORLESS,
        DARRKNESS,
        METAL,
        DRAGON,
    }
}
