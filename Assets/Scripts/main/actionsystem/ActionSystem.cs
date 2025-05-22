using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace gamecore.actionsystem
{
    public class ActionSystem
    {
        private static readonly Lazy<ActionSystem> lazy = new(() => new ActionSystem());
        public static ActionSystem INSTANCE => lazy.Value;

        private ActionSystem()
        {
            _logWriter = new AsyncGameLogWriter("action_log.json");
        }

        private readonly AsyncGameLogWriter _logWriter;
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
                var wrapperToBeRemoved = subs[type]
                    .Find(sub =>
                        sub is ActionSubscriberWrapper<T> wrapper
                        && wrapper._wrappedSubscriber == subscriber
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

        public async Task Perform(GameAction action, Action OnPerformFinished = null)
        {
            if (IsPerforming)
                return;
            IsPerforming = true;
            var logEntryBuilder = GameActionLogEntry.Builder();
            await Flow(
                action,
                logEntryBuilder,
                () =>
                {
                    IsPerforming = false;
                    _logWriter.Log(logEntryBuilder.Build());
                    OnPerformFinished?.Invoke();
                }
            );
        }

        private async Task Flow(
            GameAction action,
            GameActionLogEntry.GameActionLogEntryBuilder logEntryBuilder,
            Action OnFlowFinished = null
        )
        {
            reactions = action.PreReactions;
            action = NotifySubscribers(action, preSubs);
            await PerformReactions(logEntryBuilder);

            reactions = action.PerformReactions;
            action = await PerformAction(action);
            logEntryBuilder.WithGameAction(action);
            await PerformReactions(logEntryBuilder);

            reactions = action.PostReactions;
            NotifySubscribers(action, postSubs);
            await PerformReactions(logEntryBuilder);

            OnFlowFinished?.Invoke();
        }

        private static GameAction NotifySubscribers(
            GameAction action,
            Dictionary<Type, List<IActionSubscriber<GameAction>>> subs
        )
        {
            var type = action.GetType();
            foreach (var typedSub in subs)
            {
                if (typedSub.Key.IsAssignableFrom(type))
                {
                    foreach (var sub in typedSub.Value)
                    {
                        action = sub.React(action);
                    }
                }
            }
            return action;
        }

        private async Task PerformReactions(
            GameActionLogEntry.GameActionLogEntryBuilder logEntryBuilder
        )
        {
            foreach (var reaction in reactions)
            {
                var reactionLogEntry = GameActionLogEntry.Builder();
                logEntryBuilder.WithReaction(reactionLogEntry);
                await Flow(reaction, logEntryBuilder);
            }
            logEntryBuilder.NextTiming();
        }

        private async Task<GameAction> PerformAction(GameAction action)
        {
            if (action == null)
            {
                Debug.LogError("Attempted to perform null action");
                return null;
            }

            var type = action.GetType();

            if (performers.ContainsKey(type))
            {
                var performer = performers[type];
                if (performer == null)
                {
                    Debug.LogError($"No performer found for action type: {type.Name}");
                    return action;
                }
                return await performer.Perform(action);
            }
            Debug.LogWarning($"No performer registered for action type: {type.Name}");
            return action;
        }

        public void AddReaction(GameAction gameAction)
        {
            reactions?.Add(gameAction);
        }

        private class ActionPerformerWrapper<T> : IActionPerformer<GameAction>
            where T : GameAction
        {
            private readonly IActionPerformer<T> _wrappedPerformer;

            public ActionPerformerWrapper(IActionPerformer<T> wrappedPerformer)
            {
                _wrappedPerformer = wrappedPerformer;
            }

            public async Task<GameAction> Perform(GameAction action)
            {
                if (action is T typedAction)
                {
                    return await _wrappedPerformer.Perform(typedAction);
                }
                return action;
            }
        }

        private class ActionSubscriberWrapper<T> : IActionSubscriber<GameAction>
            where T : GameAction
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
