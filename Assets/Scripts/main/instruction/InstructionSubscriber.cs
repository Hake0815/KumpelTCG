using System;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.common;
using gamecore.game.action;

namespace gamecore.instruction
{
    class InstructionSubscriber<T>
        : IActionSubscriber<T>,
            IActionPerformer<RemoveInstructionSubscriberGA<T>>
        where T : GameAction
    {
        private readonly Func<T, bool> _reaction;
        private readonly ReactionTiming _timing;
        private readonly Guid _guid = Guid.NewGuid();
        private readonly ActionSystem _actionSystem;

        public InstructionSubscriber(
            Func<T, bool> reaction,
            ReactionTiming timing,
            ActionSystem actionSystem
        )
        {
            _reaction = reaction;
            _timing = timing;
            _actionSystem = actionSystem;
            _actionSystem.SubscribeToGameAction<T>(this, timing);
        }

        public T React(T action)
        {
            if (_reaction(action))
            {
                _actionSystem.AttachPerformer<RemoveInstructionSubscriberGA<T>>(this);
                _actionSystem.AddReaction(new RemoveInstructionSubscriberGA<T>(_guid));
            }
            return action;
        }

        public Task<RemoveInstructionSubscriberGA<T>> Perform(
            RemoveInstructionSubscriberGA<T> action
        )
        {
            if (action.Guid != _guid)
                return Task.FromResult(action);
            _actionSystem.UnsubscribeFromGameAction<T>(this, _timing);
            _actionSystem.DetachPerformer<RemoveInstructionSubscriberGA<T>>();
            return Task.FromResult(action);
        }

        public Task<RemoveInstructionSubscriberGA<T>> Reperform(
            RemoveInstructionSubscriberGA<T> action
        )
        {
            throw new IllegalStateException(
                "RemoveInstructionSubscriberGA should never be reperformed!"
            );
        }
    }
}
