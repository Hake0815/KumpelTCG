using System;
using gamecore.actionsystem;
using gamecore.game.action;

namespace gamecore.effect
{
    public class FirstTurnOfGameEffect : IPlayerEffect, IActionSubscriber<EndTurnGA>
    {
        EndTurnGA IActionSubscriber<EndTurnGA>.React(EndTurnGA action)
        {
            ActionSystem.INSTANCE.AddReaction(
                new RemovePlayerEffectGA(action.NextPlayer.Opponent, this)
            );
            return action;
        }

        public static FirstTurnOfGameEffect Create()
        {
            var effect = new FirstTurnOfGameEffect();
            ActionSystem.INSTANCE.SubscribeToGameAction(effect, ReactionTiming.POST);
            return effect;
        }

        public PlayerEffectJson ToSerializable()
        {
            return new PlayerEffectJson("first_turn_of_game");
        }
    }
}
