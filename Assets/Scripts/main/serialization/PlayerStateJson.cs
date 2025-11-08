using System;
using System.Collections.Generic;
using gamecore.effect;
using gamecore.game;

namespace gamecore.serialization
{
    [Serializable]
    public class PlayerStateJson : IJsonStringSerializable
    {
        public bool IsActive { get; }
        public bool IsAttacking { get; }
        public int HandCount { get; }
        public int DeckCount { get; }
        public int PrizesCount { get; }
        public int BenchCount { get; }
        public int DiscardPileCount { get; }
        public List<OncePerTurnActionType> PerformedOncePerTurnActions { get; }
        public int TurnCounter { get; }
        public List<PlayerEffectType> PlayerEffects { get; }

        public PlayerStateJson(
            bool isActive,
            bool isAttacking,
            int handCount,
            int deckCount,
            int prizesCount,
            int benchCount,
            int discardPileCount,
            List<OncePerTurnActionType> performedOncePerTurnActions,
            int turnCounter,
            List<PlayerEffectType> playerEffects
        )
        {
            IsActive = isActive;
            IsAttacking = isAttacking;
            HandCount = handCount;
            DeckCount = deckCount;
            PrizesCount = prizesCount;
            BenchCount = benchCount;
            DiscardPileCount = discardPileCount;
            PerformedOncePerTurnActions = performedOncePerTurnActions ?? new();
            TurnCounter = turnCounter;
            PlayerEffects = playerEffects ?? new();
        }
    }
}
