using System.Collections.Generic;

namespace gamecore.actionsystem
{
    public record GameActionLogEntry
    {
        public GameAction GameAction { get; }
        public List<GameActionLogEntry> PreReactions { get; }
        public List<GameActionLogEntry> PerformReactions { get; }
        public List<GameActionLogEntry> PostReactions { get; }

        private GameActionLogEntry(
            GameAction gameAction,
            List<GameActionLogEntry> preReactions,
            List<GameActionLogEntry> performReactions,
            List<GameActionLogEntry> postReactions
        )
        {
            GameAction = gameAction;
            PreReactions = preReactions;
            PerformReactions = performReactions;
            PostReactions = postReactions;
        }

        public static GameActionLogEntryBuilder Builder() => new();

        public class GameActionLogEntryBuilder
        {
            private GameAction _gameAction;
            private readonly List<GameActionLogEntryBuilder> _preReactions = new();
            private readonly List<GameActionLogEntryBuilder> _performReactions = new();
            private readonly List<GameActionLogEntryBuilder> _postReactions = new();
            private Timing _currentTiming = Timing.PRE;

            public GameActionLogEntryBuilder() { }

            public GameActionLogEntryBuilder WithGameAction(GameAction gameAction)
            {
                _gameAction = gameAction;
                return this;
            }

            public GameActionLogEntryBuilder WithReaction(GameActionLogEntryBuilder reaction)
            {
                switch (_currentTiming)
                {
                    case Timing.PRE:
                        _preReactions.Add(reaction);
                        break;
                    case Timing.PERFORM:
                        _performReactions.Add(reaction);
                        break;
                    case Timing.POST:
                        _postReactions.Add(reaction);
                        break;
                }
                return this;
            }

            public GameActionLogEntryBuilder NextTiming()
            {
                _currentTiming++;
                return this;
            }

            public GameActionLogEntry Build()
            {
                return new GameActionLogEntry(
                    _gameAction,
                    BuildReactions(_preReactions),
                    BuildReactions(_performReactions),
                    BuildReactions(_postReactions)
                );
            }

            private static List<GameActionLogEntry> BuildReactions(
                List<GameActionLogEntryBuilder> builders
            )
            {
                var reactions = new List<GameActionLogEntry>();
                foreach (var builder in builders)
                {
                    reactions.Add(builder.Build());
                }
                return reactions;
            }

            private enum Timing
            {
                PRE,
                PERFORM,
                POST,
            }
        }
    }
}
