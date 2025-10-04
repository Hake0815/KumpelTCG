using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.effect
{
    class PutIntoPlayThisTurnEffect : PokemonEffectAbstract, IActionSubscriber<EndTurnGA>
    {
        public PutIntoPlayThisTurnEffect(ActionSystem actionSystem, IPokemonCardLogic pokemon)
            : base(actionSystem, pokemon) { }

        public override PokemonEffectJson ToSerializable()
        {
            return new PokemonEffectJson("put_into_play_this_turn");
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
