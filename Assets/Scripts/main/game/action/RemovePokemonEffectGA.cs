using gamecore.actionsystem;
using gamecore.card;
using gamecore.effect;

namespace gamecore.game.action
{
    class RemovePokemonEffectGA : GameAction
    {
        public IPokemonCardLogic Pokemon { get; }
        public IPokemonEffect Effect { get; }

        public RemovePokemonEffectGA(IPokemonCardLogic pokemon, IPokemonEffect effect)
        {
            Pokemon = pokemon;
            Effect = effect;
        }
    }
}
