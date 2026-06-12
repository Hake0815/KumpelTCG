using System.Collections.Generic;
using System.Linq;
using gamecore.card;
using gamecore.common;
using gamecore.serialization;

namespace gamecore.game.interaction
{
    public interface IConditionalTargetQuery
    {
        public bool IsMet(List<ICard> cards);
        public bool CanBeAddedToPartialSelection(ICard candidateCard, List<ICard> partialSelection);
        public ProtoBufConditionalTargetQuery ToSerializable();
    }

    class CompoundTargetQuery : IConditionalTargetQuery
    {
        private readonly List<IConditionalTargetQuery> _queries;
        private readonly LogicalQueryOperator _logicalQueryOperator;

        public CompoundTargetQuery(
            List<IConditionalTargetQuery> queries,
            LogicalQueryOperator logicalQueryOPerator
        )
        {
            _queries = queries;
            _logicalQueryOperator = logicalQueryOPerator;
        }

        public bool CanBeAddedToPartialSelection(ICard candidateCard, List<ICard> partialSelection)
        {
            if (_queries.Count == 0)
            {
                throw new IllegalStateException("No queries provided!");
            }
            return _queries
                .Select(query => query.CanBeAddedToPartialSelection(candidateCard, partialSelection))
                .Aggregate((left, right) => _logicalQueryOperator.Apply(left, right));
        }

        public bool IsMet(List<ICard> cards)
        {
            if (_queries.Count == 0)
            {
                throw new IllegalStateException("No queries provided!");
            }
            return _queries
                .Select(query => query.IsMet(cards))
                .Aggregate((left, right) => _logicalQueryOperator.Apply(left, right));
        }

        public ProtoBufConditionalTargetQuery ToSerializable()
        {
            var protoBufConditionalTargetQuery = new ProtoBufConditionalTargetQuery
            {
                LogicalQueryOperator = _logicalQueryOperator.ToProtoBuf(),
            };
            protoBufConditionalTargetQuery.NestedQueries.Capacity = _queries.Count;
            foreach (var query in _queries)
            {
                protoBufConditionalTargetQuery.NestedQueries.Add(query.ToSerializable());
            }
            return protoBufConditionalTargetQuery;
        }
    }

    class ConditionalTargetQuery : IConditionalTargetQuery
    {
        public ConditionalTargetQuery(
            NumberRange numberRange,
            SelectionQualifier selectionQualifier
        )
        {
            _numberRange = numberRange;
            _selectionQualifier = selectionQualifier;
        }

        private readonly NumberRange _numberRange;

        private readonly SelectionQualifier _selectionQualifier;

        public bool IsMet(List<ICard> cards)
        {
            return _numberRange.Contains(_selectionQualifier.GetQualifierValue(cards));
        }

        public ProtoBufConditionalTargetQuery ToSerializable()
        {
            return new ProtoBufConditionalTargetQuery
            {
                IntRange = new ProtoBufIntRange { Min = _numberRange.Min, Max = _numberRange.Max },
                SelectionQualifier = _selectionQualifier.ToProtoBuf(),
            };
        }

        public bool CanBeAddedToPartialSelection(ICard candidateCard, List<ICard> partialSelection)
        {
            var hypotheticalSelection = new List<ICard>(partialSelection) { candidateCard };
            return _numberRange.Max >= _selectionQualifier.GetQualifierValue(hypotheticalSelection);
        }
    }

    public record NumberRange
    {
        public int Min { get; }
        public int Max { get; }

        public NumberRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public bool Contains(int value) => value >= Min && value <= Max;
    }

    public enum SelectionQualifier
    {
        NumberOfCards,
        ProvidedEnergy,
    }

    static class SelectionQualifierExtensions
    {
        public static int GetQualifierValue(this SelectionQualifier qualifier, List<ICard> cards)
        {
            return qualifier switch
            {
                SelectionQualifier.NumberOfCards => cards.Count,
                SelectionQualifier.ProvidedEnergy => cards
                    .Cast<IEnergyCardLogic>()
                    .Sum(card => card.ProvidedEnergy.Count),
                _ => throw new System.Exception($"Unknown selection qualifier: {qualifier}"),
            };
        }
    }

    public enum LogicalQueryOperator
    {
        And,
        Or,
        Xor,
    }

    static class LogicalQueryOperatorExtensions
    {
        public static bool Apply(this LogicalQueryOperator logicalOperator, bool left, bool right)
        {
            return logicalOperator switch
            {
                LogicalQueryOperator.And => left && right,
                LogicalQueryOperator.Or => left || right,
                LogicalQueryOperator.Xor => left ^ right,
                _ => throw new System.Exception($"Unknown logical operator: {logicalOperator}"),
            };
        }
    }
}
