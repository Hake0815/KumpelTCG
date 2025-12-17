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
        public bool IsLeaf { get; }
        public FilterConditionJson Condition { get; }

        public FilterJson(
            List<FilterJson> operands = null,
            FilterLogicalOperator logicalOperator = FilterLogicalOperator.None,
            bool isLeaf = false,
            FilterConditionJson condition = null
        )
        {
            Operands = operands ?? new();
            LogicalOperator = logicalOperator;
            IsLeaf = isLeaf;
            Condition = condition;
        }
    }

    public enum FilterLogicalOperator
    {
        None,
        And,
        Or,
    }

    [Serializable]
    public class FilterConditionJson : IJsonStringSerializable
    {
        public FilterType Field { get; }
        public FilterOperation Operation { get; }
        public object Value { get; }

        internal FilterConditionJson(FilterCondition condition)
        {
            Field = condition.Field;
            Operation = condition.Operation;
            Value = condition.Value ?? -1;
        }
    }
}
