using System;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.common;
using gamecore.game.action;
using UnityEngine;

namespace gamecore.effect
{
    class EffectSubscriber<T> : IActionSubscriber<T>, IActionPerformer<RemoveEffectSubscriberGA<T>>
        where T : GameAction
    {
        private readonly Func<T, bool> _reaction;
        private readonly ReactionTiming _timing;

        public EffectSubscriber(Func<T, bool> reaction, ReactionTiming timing)
        {
            _reaction = reaction;
            _timing = timing;
            ActionSystem.INSTANCE.SubscribeToGameAction<T>(this, timing);
        }

        public T React(T action)
        {
            if (_reaction(action))
            {
                ActionSystem.INSTANCE.AttachPerformer<RemoveEffectSubscriberGA<T>>(this);
                ActionSystem.INSTANCE.AddReaction(new RemoveEffectSubscriberGA<T>());
            }
            return action;
        }

        public Task<RemoveEffectSubscriberGA<T>> Perform(RemoveEffectSubscriberGA<T> action)
        {
            ActionSystem.INSTANCE.UnsubscribeFromGameAction<T>(this, _timing);
            ActionSystem.INSTANCE.DetachPerformer<RemoveEffectSubscriberGA<T>>();
            return Task.FromResult(action);
        }

        public Task<RemoveEffectSubscriberGA<T>> Reperform(RemoveEffectSubscriberGA<T> action)
        {
            throw new IlleagalStateException(
                "RemoveEffectSubscriberGA should never be reperformed!"
            );
        }
    }
}
