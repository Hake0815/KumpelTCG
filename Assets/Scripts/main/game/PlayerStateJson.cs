using System;
using System.Collections.Generic;
using gamecore.effect;

namespace gamecore.game
{
    [Serializable]
    public class PlayerStateJson
    {
        public bool IsActive { get; }
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
