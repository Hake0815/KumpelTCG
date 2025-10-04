using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.effect
{
    class AbilityUsedThisTurnEffect : PokemonEffectAbstract, IActionSubscriber<EndTurnGA>
    {
        public AbilityUsedThisTurnEffect(ActionSystem actionSystem, IPokemonCardLogic pokemon)
            : base(actionSystem, pokemon) { }

        public override PokemonEffectJson ToSerializable()
        {
            return new PokemonEffectJson("ability_used_this_turn");
        }

        internal override void Apply()
        {
            _pokemon.AddEffect(this);
            _actionSystem.SubscribeToGameAction(this, ReactionTiming.POST);
        }

        EndTurnGA IActionSubscriber<EndTurnGA>.React(EndTurnGA action)
        {
            _actionSystem.AddReaction(new RemovePokemonEffectGA(_pokemon, this));
            return action;
        }
    }
}
