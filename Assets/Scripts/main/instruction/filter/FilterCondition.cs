using System;
using System.Collections.Generic;
using gamecore.card;
using gamecore.serialization;

namespace gamecore.instruction.filter
{
    [Serializable]
    class FilterCondition : FilterNode
    {
        public FilterCondition(FilterType field, FilterOperation operation, object value)
        {
            Field = field;
            Operation = operation;
            Value = value;
        }

        public FilterCondition(FilterType field)
            : this(field, FilterOperation.None, null)
        {
            if (field is not FilterType.True && field is not FilterType.ExcludeSource)
                throw new ArgumentException(
                    $"FilterCondition of type {field} needs to have a operation and value"
                );
        }

        public FilterType Field { get; }
        public FilterOperation Operation { get; }
        public object Value { get; }

        public override bool Matches(ICardLogic card, ICardLogic sourceCard)
        {
            return Field switch
            {
                FilterType.True => true,
                FilterType.ExcludeSource => card != sourceCard,
                FilterType.CardType => Compare(card.CardType, Operation, Value),
                FilterType.CardSubtype => Compare(card.CardSubtype, Operation, Value),
                FilterType.Hp => card is IPokemonCardLogic pokemon
                    && Compare(pokemon.MaxHP, Operation, Value),
                _ => false,
            };
        }

        private static bool Compare<T>(T actual, FilterOperation op, object expected)
        {
            if (actual is not IComparable a || expected is not IComparable b)
                return false;

            int cmp = a.CompareTo(b);
            return op switch
            {
                FilterOperation.Equals => cmp == 0,
                FilterOperation.NotEquals => cmp != 0,
                FilterOperation.LessThanOrEqual => cmp <= 0,
                FilterOperation.GreaterThanOrEqual => cmp >= 0,
                _ => false,
            };
        }

        public override ProtoBufFilter ToSerializable()
        {
            var protobufFilterType = Field switch
            {
                FilterType.None => ProtoBufFilterType.FilterTypeNone,
                FilterType.True => ProtoBufFilterType.FilterTypeTrue,
                FilterType.ExcludeSource => ProtoBufFilterType.FilterTypeExcludeSource,
                FilterType.CardType => ProtoBufFilterType.FilterTypeCardType,
                FilterType.CardSubtype => ProtoBufFilterType.FilterTypeCardSubtype,
                FilterType.Hp => ProtoBufFilterType.FilterTypeHp,
                _ => throw new System.NotImplementedException(),
            };
            var protobufFilterOperation = Operation switch
            {
                FilterOperation.None => ProtoBufFilterOperation.FilterOperationNone,
                FilterOperation.Equals => ProtoBufFilterOperation.FilterOperationEquals,
                FilterOperation.NotEquals => ProtoBufFilterOperation.FilterOperationNotEquals,
                FilterOperation.LessThanOrEqual =>
                    ProtoBufFilterOperation.FilterOperationLessThanOrEqual,
                FilterOperation.GreaterThanOrEqual =>
                    ProtoBufFilterOperation.FilterOperationGreaterThanOrEqual,
                _ => throw new System.NotImplementedException(),
            };

            return new ProtoBufFilter
            {
                IsLeaf = true,
                Condition = new ProtoBufFilterCondition
                {
                    Field = protobufFilterType,
                    Operation = protobufFilterOperation,
                    Value = (int)Value,
                },
            };
        }
    }
}
