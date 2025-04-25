using System;
using gamecore.actionsystem;
using gamecore.game.action;

namespace gamecore.action
{
    internal class AttackSystem : IActionPerformer<AttackGA>, IActionSubscriber<AttackGA>
    {
        private static readonly Lazy<AttackSystem> lazy = new(() => new AttackSystem());
        public static AttackSystem INSTANCE => lazy.Value;

        private AttackSystem() { }

        private readonly ActionSystem _actionSystem = ActionSystem.INSTANCE;

        public void Enable()
        {
            _actionSystem.AttachPerformer<AttackGA>(INSTANCE);
            _actionSystem.SubscribeToGameAction<AttackGA>(INSTANCE, ReactionTiming.POST);
        }

        public void Disable()
        {
            _actionSystem.DetachPerformer<AttackGA>();
            _actionSystem.UnsubscribeFromGameAction<AttackGA>(INSTANCE, ReactionTiming.POST);
        }

        public AttackGA Perform(AttackGA action)
        {
            foreach (var effect in action.Attack.Effects)
            {
                effect.Perform(action.Attacker);
            }
            return action;
        }

        public AttackGA React(AttackGA action)
        {
            _actionSystem.AddReaction(new EndTurnGA());
            return action;
        }
    }
}
