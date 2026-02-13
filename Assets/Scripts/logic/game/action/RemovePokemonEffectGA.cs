using gamecore.actionsystem;
using gamecore.card;
using gamecore.effect;

namespace gamecore.game.action
{
    class RemovePokemonEffectGA : GameAction
    {
        public IPokemonCardLogic Pokemon { get; }
        public PokemonEffectAbstract Effect { get; }

        public RemovePokemonEffectGA(IPokemonCardLogic pokemon, PokemonEffectAbstract effect)
        {
            Pokemon = pokemon;
            Effect = effect;
        }
    }
}
