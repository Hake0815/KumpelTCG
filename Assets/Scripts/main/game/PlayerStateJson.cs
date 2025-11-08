using System;
using System.Collections.Generic;
using gamecore.common;
using gamecore.effect;

namespace gamecore.game
{
    [Serializable]
    public class PlayerStateJson : JsonStringSerializable
    {
        public bool IsActive { get; }
        public bool IsAttacking { get; }
        public int HandCount { get; }
        public int DeckCount { get; }
        public int PrizesCount { get; }
        public int BenchCount { get; }
        public int DiscardPileCount { get; }
        public List<string> PerformedOncePerTurnActions { get; }
        public int TurnCounter { get; }
        public List<PlayerEffectJson> PlayerEffects { get; }

        public PlayerStateJson(
            bool isActive,
            bool isAttacking,
            int handCount,
            int deckCount,
            int prizesCount,
            int benchCount,
            int discardPileCount,
            List<string> performedOncePerTurnActions,
            int turnCounter,
            List<PlayerEffectJson> playerEffects
        )
        {
            IsActive = isActive;
            IsAttacking = isAttacking;
            HandCount = handCount;
            DeckCount = deckCount;
            PrizesCount = prizesCount;
            BenchCount = benchCount;
            DiscardPileCount = discardPileCount;
            PerformedOncePerTurnActions = performedOncePerTurnActions ?? new List<string>();
            TurnCounter = turnCounter;
            PlayerEffects = playerEffects ?? new List<PlayerEffectJson>();
        }
    }
}
