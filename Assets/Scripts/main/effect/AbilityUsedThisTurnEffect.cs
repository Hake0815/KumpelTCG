using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.effect
{
    public class AbilityUsedThisTurnEffect : IPokemonEffect, IActionSubscriber<EndTurnGA>
    {
        private IPokemonCardLogic _pokemon;

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
