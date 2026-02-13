using System;
using gamecore.serialization;

namespace gamecore.card
{
    public enum PositionKnowledge
    {
        Unknown = 0,
        NotPrized = 1,
        Known = 2,
    }

    public static class PositionKnowledgeExtensions
    {
        public static PositionKnowledge InformationLost(this PositionKnowledge positionKnowledge)
        {
            return positionKnowledge switch
            {
                PositionKnowledge.Unknown => PositionKnowledge.Unknown,
                PositionKnowledge.NotPrized => PositionKnowledge.NotPrized,
                PositionKnowledge.Known => PositionKnowledge.NotPrized,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(positionKnowledge),
                    $"Not a valid position knowledge: {positionKnowledge}"
                ),
            };
        }

        public static ProtoBufPositionKnowledge ToProtoBuf(this PositionKnowledge positionKnowledge)
        {
            return positionKnowledge switch
            {
                PositionKnowledge.Unknown => ProtoBufPositionKnowledge.PositionKnowledgeUnknown,
                PositionKnowledge.NotPrized => ProtoBufPositionKnowledge.PositionKnowledgeNotPrized,
                PositionKnowledge.Known => ProtoBufPositionKnowledge.PositionKnowledgeKnown,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(positionKnowledge),
                    $"Not a valid position knowledge: {positionKnowledge}"
                ),
            };
        }

        public static PositionKnowledge FromProtoBuf(
            this ProtoBufPositionKnowledge positionKnowledge
        )
        {
            return positionKnowledge switch
            {
                ProtoBufPositionKnowledge.PositionKnowledgeUnknown => PositionKnowledge.Unknown,
                ProtoBufPositionKnowledge.PositionKnowledgeNotPrized => PositionKnowledge.NotPrized,
                ProtoBufPositionKnowledge.PositionKnowledgeKnown => PositionKnowledge.Known,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(positionKnowledge),
                    $"Not a valid position knowledge: {positionKnowledge}"
                ),
            };
        }
    }
}
