using System;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.common;
using gamecore.game.action;
using UnityEngine;

namespace gamecore.instruction
{
    class InstructionSubscriber<T>
        : IActionSubscriber<T>,
            IActionPerformer<RemoveInstructionSubscriberGA<T>>
        where T : GameAction
    {
        private readonly Func<T, bool> _reaction;
        private readonly ReactionTiming _timing;

        public InstructionSubscriber(Func<T, bool> reaction, ReactionTiming timing)
        {
            _reaction = reaction;
            _timing = timing;
            ActionSystem.INSTANCE.SubscribeToGameAction<T>(this, timing);
        }

        public T React(T action)
        {
            if (_reaction(action))
            {
                ActionSystem.INSTANCE.AttachPerformer<RemoveInstructionSubscriberGA<T>>(this);
                ActionSystem.INSTANCE.AddReaction(new RemoveInstructionSubscriberGA<T>());
            }
            return action;
        }

        public Task<RemoveInstructionSubscriberGA<T>> Perform(
            RemoveInstructionSubscriberGA<T> action
        )
        {
            ActionSystem.INSTANCE.UnsubscribeFromGameAction<T>(this, _timing);
            ActionSystem.INSTANCE.DetachPerformer<RemoveInstructionSubscriberGA<T>>();
            return Task.FromResult(action);
        }

        public Task<RemoveInstructionSubscriberGA<T>> Reperform(
            RemoveInstructionSubscriberGA<T> action
        )
        {
            throw new IlleagalStateException(
                "RemoveInstructionSubscriberGA should never be reperformed!"
            );
        }
    }
}
