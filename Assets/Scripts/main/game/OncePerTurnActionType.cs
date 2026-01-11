using System;
using gamecore.serialization;

namespace gamecore.game
{
    public enum OncePerTurnActionType
    {
        AttachedEnergyForTurn,
        PlayedSupporterThisTurn,
        Retreated,
    }

    public static class ToProtoBufExtensions
    {
        public static ProtoBufOncePerTurnActionType ToProtoBuf(
            this OncePerTurnActionType oncePerTurnActionType
        )
        {
            return oncePerTurnActionType switch
            {
                OncePerTurnActionType.AttachedEnergyForTurn =>
                    ProtoBufOncePerTurnActionType.OncePerTurnActionTypeAttachedEnergyForTurn,
                OncePerTurnActionType.PlayedSupporterThisTurn =>
                    ProtoBufOncePerTurnActionType.OncePerTurnActionTypePlayedSupporterThisTurn,
                OncePerTurnActionType.Retreated =>
                    ProtoBufOncePerTurnActionType.OncePerTurnActionTypeRetreated,
                _ => throw new InvalidOperationException(
                    $"Invalid once per turn action type: {oncePerTurnActionType}"
                ),
            };
        }
    }
}
