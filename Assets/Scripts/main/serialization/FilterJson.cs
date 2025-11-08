using System;
using System.Collections.Generic;
using gamecore.instruction.filter;

namespace gamecore.serialization
{
    [Serializable]
    public class FilterJson : IJsonStringSerializable
    {
        public List<FilterJson> Operands { get; }
        public FilterLogicalOperator LogicalOperator { get; }
        public LeafType LeafType { get; }
        public FilterConditionJson Condition { get; }

        public FilterJson(
            List<FilterJson> operands = null,
            FilterLogicalOperator logicalOperator = FilterLogicalOperator.None,
            LeafType leafType = LeafType.None,
            FilterConditionJson condition = null
        )
        {
            Operands = operands ?? new();
            LogicalOperator = logicalOperator;
            LeafType = leafType;
            Condition = condition;
        }
    }

    public enum FilterLogicalOperator
    {
        None,
        And,
        Or,
    }

    public enum LeafType
    {
        None,
        True,
        ExcludeSource,
        Condition,
    }

    [Serializable]
    public class FilterConditionJson : IJsonStringSerializable
    {
        public FilterAttribute Field { get; }
        public FilterOperation Operation { get; }
        public object Value { get; }

        public FilterConditionJson(FilterAttribute field, FilterOperation operation, object value)
        {
            Field = field;
            Operation = operation;
            Value = value;
        }

        internal FilterConditionJson(FilterCondition condition)
        {
            Field = condition.Field;
            Operation = condition.Operation;
            Value = condition.Value;
        }
    }
}
