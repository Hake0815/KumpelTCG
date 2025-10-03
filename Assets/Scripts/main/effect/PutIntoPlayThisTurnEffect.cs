using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.effect
{
    public class PutIntoPlayThisTurnEffect : IPokemonEffect, IActionSubscriber<EndTurnGA>
    {
        private IPokemonCardLogic _pokemon;

        public PokemonEffectJson ToSerializable()
        {
            return new PokemonEffectJson("put_into_play_this_turn");
        }

        void IPokemonEffect.Apply(IPokemonCardLogic pokemon)
        {
            _pokemon = pokemon;
            pokemon.AddEffect(this);
            ActionSystem.INSTANCE.SubscribeToGameAction(this, ReactionTiming.POST);
        }

        EndTurnGA IActionSubscriber<EndTurnGA>.React(EndTurnGA action)
        {
            ActionSystem.INSTANCE.AddReaction(new RemovePokemonEffectGA(_pokemon, this));
            return action;
        }
    }
}
