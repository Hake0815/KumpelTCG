using System;

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
    }
}
