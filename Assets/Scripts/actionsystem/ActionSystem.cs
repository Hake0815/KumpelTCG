using System;
using System.Collections;
using System.Collections.Generic;
namespace gamecore.actionsystem
{
    public class ActionSystem
    {
        private static readonly Lazy<ActionSystem> lazy = new(() => new ActionSystem());
        public static ActionSystem INSTANCE => lazy.Value;

        private ActionSystem() { }

        private List<GameAction> reactions = null;
        public bool IsPerforming { get; private set; } = false;
        private readonly Dictionary<Type, List<IActionSubscriber<GameAction>>> preSubs = new();
        private readonly Dictionary<Type, List<IActionSubscriber<GameAction>>> postSubs = new();
        private readonly Dictionary<Type, IActionPerformer<GameAction>> performers = new();

        public void AttachPerformer<T>(IActionPerformer<T> performer)
            where T : GameAction
        {
            var type = typeof(T);
            var wrappedPerformer = new ActionPerformerWrapper<T>(performer);
            performers[type] = wrappedPerformer;
        }

        public void DetachPerformer<T>()
            where T : GameAction
        {
            var type = typeof(T);
            if (performers.ContainsKey(type))
                performers.Remove(type);
        }

        public void SubscribeToGameAction<T>(IActionSubscriber<T> subscriber, ReactionTiming timing)
            where T : GameAction
        {
            var type = typeof(T);
            var subs = GetTimingSubscribers(timing);
            if (!subs.ContainsKey(type))
                subs.Add(type, new());
            var wrappedSubscriber = new ActionSubscriberWrapper<T>(subscriber);
            subs[type].Add(wrappedSubscriber);
        }

        public void UnsubscribeFromGameAction<T>(
            IActionSubscriber<T> subscriber,
            ReactionTiming timing
        )
            where T : GameAction
        {
            var type = typeof(T);
            var subs = GetTimingSubscribers(timing);
            if (subs.ContainsKey(type))
            {
                // Find and remove the wrapper that contains this subscriber
                var wrapperToBeRemoved = subs[type].Find(sub =>
                    sub is ActionSubscriberWrapper<T> wrapper &&
                    wrapper._wrappedSubscriber == subscriber
                );

                if (wrapperToBeRemoved != null)
                {
                    subs[type].Remove(wrapperToBeRemoved);
                }
            }
        }

        private Dictionary<Type, List<IActionSubscriber<GameAction>>> GetTimingSubscribers(
            ReactionTiming timing
        )
        {
            return timing == ReactionTiming.PRE ? preSubs : postSubs;
        }

        public void Perform(GameAction action, Action OnPerformFinished = null)
        {
            if (IsPerforming)
                return;
            IsPerforming = true;
            Flow(
                action,
                () =>
                {
                    IsPerforming = false;
                    OnPerformFinished?.Invoke();
                }
            );
        }

        private void Flow(GameAction action, Action OnFlowFinished = null)
        {
            reactions = action.PreReactions;
            action = NotifySubscribers(action, preSubs);
            PerformReactions();

            reactions = action.PerformReactions;
            action = PerformAction(action);
            PerformReactions();

            reactions = action.PostReactions;
            NotifySubscribers(action, postSubs);
            PerformReactions();

            OnFlowFinished?.Invoke();
        }

        private GameAction NotifySubscribers(
            GameAction action,
            Dictionary<Type, List<IActionSubscriber<GameAction>>> subs
        )
        {
            var type = action.GetType();
            if (subs.ContainsKey(type))
            {
                foreach (var sub in subs[type])
                {
                    action = sub.React(action);
                }
            }
            return action;
        }

        private void PerformReactions()
        {
            foreach (var reaction in reactions)
            {
                Flow(reaction);
            }
        }

        private GameAction PerformAction(GameAction action)
        {
            var type = action.GetType();
            if (performers.ContainsKey(type))
            {
                return performers[type].Perform(action);
            }
            return action;
        }

        public void AddReaction(GameAction gameAction)
        {
            reactions?.Add(gameAction);
        }

        private class ActionPerformerWrapper<T> : IActionPerformer<GameAction> where T : GameAction
        {
            private readonly IActionPerformer<T> _wrappedPerformer;

            public ActionPerformerWrapper(IActionPerformer<T> wrappedPerformer)
            {
                _wrappedPerformer = wrappedPerformer;
            }

            public GameAction Perform(GameAction action)
            {
                if (action is T typedAction)
                {
                    return _wrappedPerformer.Perform(typedAction);
                }
                return action;
            }
        }

        private class ActionSubscriberWrapper<T> : IActionSubscriber<GameAction> where T : GameAction
        {
            internal readonly IActionSubscriber<T> _wrappedSubscriber;

            public ActionSubscriberWrapper(IActionSubscriber<T> wrappedSubscriber)
            {
                _wrappedSubscriber = wrappedSubscriber;
            }

            public GameAction React(GameAction action)
            {
                if (action is T typedAction)
                {
                    return _wrappedSubscriber.React(typedAction);
                }
                return action;
            }
        }
    }
}
