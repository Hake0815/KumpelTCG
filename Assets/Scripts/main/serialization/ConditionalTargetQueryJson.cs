using System;
using System.Collections.Generic;
using gamecore.game.interaction;

namespace gamecore.serialization
{
    [Serializable]
    public class ConditionalTargetQueryJson : IJsonStringSerializable
    {
        public List<ConditionalTargetQueryJson> NestedQueryJsons { get; }
        public LogicalQueryOperator? LogicalQueryOperator { get; }
        public NumberRange NumberRange { get; }
        public SelectionQualifier? SelectionQualifier { get; }

        public ConditionalTargetQueryJson(
            NumberRange numberRange,
            SelectionQualifier? selectionQualifier
        )
        {
            NumberRange = numberRange;
            SelectionQualifier = selectionQualifier;
        }

        public ConditionalTargetQueryJson(
            List<ConditionalTargetQueryJson> nestedQueryJsons,
            LogicalQueryOperator? logicalQueryOperator
        )
        {
            NestedQueryJsons = nestedQueryJsons;
            LogicalQueryOperator = logicalQueryOperator;
        }
    }
}
