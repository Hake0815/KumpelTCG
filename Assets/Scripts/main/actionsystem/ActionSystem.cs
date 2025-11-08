using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using gamecore.common;
using gamecore.game.action;
using Newtonsoft.Json;

namespace gamecore.actionsystem
{
    public class ActionSystem
    {
        public ActionSystem(string logFilePath)
        {
            _logWriter = new AsyncGameLogWriter(logFilePath);
        }

        private readonly AsyncGameLogWriter _logWriter;
        private List<GameAction> _reactions = null;
        public bool IsPerforming { get; private set; } = false;
        private readonly Dictionary<Type, List<IActionSubscriber<GameAction>>> _preSubs = new();
        private readonly Dictionary<Type, List<IActionSubscriber<GameAction>>> _postSubs = new();
        private readonly Dictionary<Type, IActionPerformer<GameAction>> _performers = new();
        private int _replayedActions = 0;

        public void FinishGameLog()
        {
            _logWriter.FinishLog();
        }

        public void AttachPerformer<T>(IActionPerformer<T> performer)
            where T : GameAction
        {
            var type = typeof(T);
            var wrappedPerformer = new ActionPerformerWrapper<T>(performer);
            _performers[type] = wrappedPerformer;
        }

        public void DetachPerformer<T>()
            where T : GameAction
        {
            var type = typeof(T);
            if (_performers.ContainsKey(type))
                _performers.Remove(type);
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
            return timing == ReactionTiming.PRE ? _preSubs : _postSubs;
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
                    var entry = logEntryBuilder.Build();
                    GlobalLogger.Instance.Debug(JsonConvert.SerializeObject(entry));
                    _logWriter.Log(entry);
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
            _reactions = action.PreReactions;
            action = NotifySubscribers(action, _preSubs);
            await PerformReactions(logEntryBuilder);

            _reactions = action.PerformReactions;
            action = await PerformAction(action);
            logEntryBuilder.WithGameAction(action);
            await PerformReactions(logEntryBuilder);

            _reactions = action.PostReactions;
            NotifySubscribers(action, _postSubs);
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
            foreach (var reaction in _reactions)
            {
                var reactionLogEntry = GameActionLogEntry.Builder();
                logEntryBuilder.WithReaction(reactionLogEntry);
                await Flow(reaction, reactionLogEntry);
            }
            logEntryBuilder.NextTiming();
        }

        private async Task<GameAction> PerformAction(GameAction action)
        {
            if (action == null)
            {
                GlobalLogger.Instance.Error("ERROR: Attempted to perform null action");
                return null;
            }

            var type = action.GetType();

            if (_performers.ContainsKey(type))
            {
                var performer = _performers[type];
                if (performer == null)
                {
                    GlobalLogger.Instance.Error(
                        $"ERROR: No performer found for action type: {type.Name}"
                    );
                    return action;
                }
                return await performer.Perform(action);
            }
            GlobalLogger.Instance.Warning(
                $"WARNING: No performer registered for action type: {type.Name}"
            );
            return action;
        }

        public void AddReaction(GameAction gameAction)
        {
            _reactions?.Add(gameAction);
        }

        public async Task RecreateGameStateFromLog()
        {
            var logEntries = _logWriter.LoadExistingLog();
            foreach (var logEntry in logEntries)
            {
                await Reperform(logEntry);
            }
        }

        public async Task<bool> ReplayNextAction()
        {
            var logEntries = _logWriter.LoadExistingLog();
            await Reperform(logEntries[_replayedActions]);
            _replayedActions++;
            return _replayedActions < logEntries.Count;
        }

        private async Task Reperform(GameActionLogEntry logEntry)
        {
            foreach (var preReaction in logEntry.PreReactions)
            {
                await Reperform(preReaction);
            }
            await ReperformAction(logEntry.GameAction);
            foreach (var performReaction in logEntry.PerformReactions)
            {
                await Reperform(performReaction);
            }
            foreach (var postReaction in logEntry.PostReactions)
            {
                await Reperform(postReaction);
            }
        }

        private async Task ReperformAction(GameAction action)
        {
            if (action == null)
            {
                GlobalLogger.Instance.Error("ERROR: Attempted to reperform null action");
                return;
            }

            var type = action.GetType();
            if (type == typeof(RemoveInstructionSubscriberGA))
                return;

            if (_performers.ContainsKey(type))
            {
                var performer = _performers[type];
                if (performer == null)
                    GlobalLogger.Instance.Error(
                        $"ERROR: No reperformer found for action type: {type.Name}"
                    );
                else
                    await performer.Reperform(action);
            }
            else
                GlobalLogger.Instance.Warning(
                    $"WARNING: No reperformer registered for action type: {type.Name}"
                );
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

            async Task<GameAction> IActionPerformer<GameAction>.Reperform(GameAction action)
            {
                if (action is T typedAction)
                {
                    return await _wrappedPerformer.Reperform(typedAction);
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
