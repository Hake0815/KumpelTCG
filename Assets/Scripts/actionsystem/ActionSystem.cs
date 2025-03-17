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
        private readonly Dictionary<Type, List<Func<GameAction, GameAction>>> preSubs = new();
        private readonly Dictionary<Type, List<Func<GameAction, GameAction>>> postSubs = new();
        private readonly Dictionary<Type, Func<GameAction, GameAction>> performers = new();

        public void AttachPerformer<T>(Func<T, T> performer)
            where T : GameAction
        {
            var type = typeof(T);
            GameAction wrappedPerformer(GameAction action) => performer((T)action);
            performers[type] = wrappedPerformer;
        }

        public void DetachPerformer<T>()
            where T : GameAction
        {
            var type = typeof(T);
            if (performers.ContainsKey(type))
                performers.Remove(type);
        }

        public void SubscribeReaction<T>(Func<T, T> reaction, ReactionTiming timing)
            where T : GameAction
        {
            var type = typeof(T);
            var subs = GetTimingSubscribers(timing);
            GameAction wrappedReaction(GameAction action) => reaction((T)action);
            if (!subs.ContainsKey(type))
                subs.Add(type, new());
            subs[type].Add(wrappedReaction);
        }

        public void UnsubscribeReaction<T>(Func<T, T> reaction, ReactionTiming timing)
            where T : GameAction
        {
            var type = typeof(T);
            var subs = GetTimingSubscribers(timing);
            if (subs.ContainsKey(type))
            {
                GameAction wrappedReaction(GameAction action) => reaction((T)action);
                subs[type].Remove(wrappedReaction);
            }
        }

        private Dictionary<Type, List<Func<GameAction, GameAction>>> GetTimingSubscribers(
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
            action = PerformSubscribers(action, preSubs);
            PerformReactions();

            reactions = action.PerformReactions;
            action = PerformPerformer(action);
            PerformReactions();

            reactions = action.PostReactions;
            PerformSubscribers(action, postSubs);
            PerformReactions();

            OnFlowFinished?.Invoke();
        }

        private GameAction PerformSubscribers(
            GameAction action,
            Dictionary<Type, List<Func<GameAction, GameAction>>> subs
        )
        {
            var type = action.GetType();
            if (subs.ContainsKey(type))
            {
                foreach (var sub in subs[type])
                {
                    action = sub(action);
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

        private GameAction PerformPerformer(GameAction action)
        {
            var type = action.GetType();
            if (performers.ContainsKey(type))
            {
                action = performers[type](action);
            }
            return action;
        }

        public void AddReaction(GameAction gameAction)
        {
            reactions?.Add(gameAction);
        }
    }
}
