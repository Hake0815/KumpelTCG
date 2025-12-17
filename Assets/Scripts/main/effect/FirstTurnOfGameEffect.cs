using System;
using gamecore.actionsystem;
using gamecore.game.action;
using gamecore.serialization;

namespace gamecore.effect
{
    public class FirstTurnOfGameEffect : PlayerEffectAbstract, IActionSubscriber<EndTurnGA>
    {
        public FirstTurnOfGameEffect(ActionSystem actionSystem)
            : base(actionSystem) { }

        EndTurnGA IActionSubscriber<EndTurnGA>.React(EndTurnGA action)
        {
            _actionSystem.AddReaction(new RemovePlayerEffectGA(action.NextPlayer.Opponent, this));
            return action;
        }

        public static FirstTurnOfGameEffect Create(ActionSystem actionSystem)
        {
            var effect = new FirstTurnOfGameEffect(actionSystem);
            actionSystem.SubscribeToGameAction(effect, ReactionTiming.POST);
            return effect;
        }

        public override PlayerEffectType ToSerializable()
        {
            return PlayerEffectType.FirstTurnOfGame;
        }
    }
}
