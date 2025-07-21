using System;
using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using gamecore.card;

namespace gamecore.instruction.filter
{
    [Serializable]
    class FilterCondition : FilterNode
    {
        public FilterCondition(FilterAttribute field, FilterOperation operation, object value)
        {
            Field = field;
            Operation = operation;
            Value = value;
        }

        public FilterAttribute Field { get; }
        public FilterOperation Operation { get; }
        public object Value { get; }

        public override bool Matches(ICardLogic card, ICardLogic sourceCard)
        {
            return Field switch
            {
                FilterAttribute.CardType => Compare(card.CardType, Operation, Value),
                FilterAttribute.CardSubtype => Compare(card.CardSubtype, Operation, Value),
                FilterAttribute.Hp => card is IPokemonCardLogic pokemon
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

        public override object ToSerializable()
        {
            return new Dictionary<string, object>
            {
                { "op", "condition" },
                {
                    "condition",
                    new Dictionary<string, object>
                    {
                        { "field", Field.ToString() },
                        { "op", Operation.ToString() },
                        { "value", Value.ToString() },
                    }
                },
            };
        }
    }
}
