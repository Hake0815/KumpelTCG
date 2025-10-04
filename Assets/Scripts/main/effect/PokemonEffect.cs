using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.effect
{
    public abstract class PokemonEffectAbstract
    {
        private protected readonly ActionSystem _actionSystem;
        private protected readonly IPokemonCardLogic _pokemon;

        private protected PokemonEffectAbstract(
            ActionSystem actionSystem,
            IPokemonCardLogic pokemon
        )
        {
            _actionSystem = actionSystem;
            _pokemon = pokemon;
        }

        internal abstract void Apply();
        public abstract PokemonEffectJson ToSerializable();
    }
}
