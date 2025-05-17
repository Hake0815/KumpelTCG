using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.game.action;

namespace gamecore.effect
{
    class EffectSubscriber<T> : IActionSubscriber<T>, IActionPerformer<RemoveEffectSubscriberGA<T>>
        where T : GameAction
    {
        public Func<T, T> Reaction { get; }
        public ReactionTiming Timing { get; }

        public EffectSubscriber(Func<T, T> reaction, ReactionTiming timing)
        {
            Reaction = reaction;
            Timing = timing;
            ActionSystem.INSTANCE.SubscribeToGameAction<T>(this, timing);
            ActionSystem.INSTANCE.AttachPerformer<RemoveEffectSubscriberGA<T>>(this);
        }

        public T React(T action)
        {
            action = Reaction(action);
            ActionSystem.INSTANCE.AddReaction(new RemoveEffectSubscriberGA<T>());
            return action;
        }

        public Task<RemoveEffectSubscriberGA<T>> Perform(RemoveEffectSubscriberGA<T> action)
        {
            ActionSystem.INSTANCE.UnsubscribeFromGameAction<T>(this, Timing);
            ActionSystem.INSTANCE.DetachPerformer<RemoveEffectSubscriberGA<T>>();
            return Task.FromResult(action);
        }
    }
}
