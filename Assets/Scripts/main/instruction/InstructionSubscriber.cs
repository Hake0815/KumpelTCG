using System;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.common;
using gamecore.game.action;

namespace gamecore.instruction
{
    class InstructionSubscriber<T> : IActionSubscriber<T>
        where T : GameAction
    {
        private readonly Func<T, bool> _reaction;
        private readonly ReactionTiming _timing;
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
                _actionSystem.AddReaction(
                    new RemoveInstructionSubscriberGA(
                        () => _actionSystem.UnsubscribeFromGameAction<T>(this, _timing)
                    )
                );
            }
            return action;
        }
    }
}
