using System;
using gamecore.serialization;

namespace gamecore.game
{
    public enum PlayerTurnTrait
    {
        FirstTurnOfGame,
        AttachedEnergyForTurn,
        PlayedSupporterThisTurn,
        Retreated,
    }

    public static class ToProtoBufPlayerTurnTraitExtensions
    {
        public static ProtoBufPlayerTurnTrait ToProtoBuf(this PlayerTurnTrait playerTurnTrait)
        {
            return playerTurnTrait switch
            {
                PlayerTurnTrait.FirstTurnOfGame =>
                    ProtoBufPlayerTurnTrait.PlayerTurnTraitFirstTurnOfGame,
                PlayerTurnTrait.AttachedEnergyForTurn =>
                    ProtoBufPlayerTurnTrait.PlayerTurnTraitAttachedEnergyForTurn,
                PlayerTurnTrait.PlayedSupporterThisTurn =>
                    ProtoBufPlayerTurnTrait.PlayerTurnTraitPlayedSupporterThisTurn,
                PlayerTurnTrait.Retreated => ProtoBufPlayerTurnTrait.PlayerTurnTraitRetreated,
                _ => throw new InvalidOperationException(
                    $"Invalid player turn trait: {playerTurnTrait}"
                ),
            };
        }

        public static PlayerTurnTrait FromProtoBuf(
            this ProtoBufPlayerTurnTrait protoBufPlayerTurnTrait
        )
        {
            return protoBufPlayerTurnTrait switch
            {
                ProtoBufPlayerTurnTrait.PlayerTurnTraitFirstTurnOfGame =>
                    PlayerTurnTrait.FirstTurnOfGame,
                ProtoBufPlayerTurnTrait.PlayerTurnTraitAttachedEnergyForTurn =>
                    PlayerTurnTrait.AttachedEnergyForTurn,
                ProtoBufPlayerTurnTrait.PlayerTurnTraitPlayedSupporterThisTurn =>
                    PlayerTurnTrait.PlayedSupporterThisTurn,
                ProtoBufPlayerTurnTrait.PlayerTurnTraitRetreated => PlayerTurnTrait.Retreated,
                _ => throw new InvalidOperationException(
                    $"Invalid player turn trait: {protoBufPlayerTurnTrait}"
                ),
            };
        }
    }
}
