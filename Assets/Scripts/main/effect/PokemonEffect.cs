using gamecore.card;

namespace gamecore.effect
{
    public interface IPokemonEffect
    {
        internal void Apply(IPokemonCardLogic pokemon);
        PokemonEffectJson ToSerializable();
    }
}
