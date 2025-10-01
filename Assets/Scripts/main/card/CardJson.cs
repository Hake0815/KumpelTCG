using System;
using System.Collections.Generic;

namespace gamecore.card
{
    [Serializable]
    public class CardJson
    {
        public CardDataJson CardData { get; }
        public int DeckId { get; }

        // Pokemon-specific current state
        public int CurrentDamage { get; }
        public List<int> AttachedEnergy { get; }
        public List<int> PreEvolutionIds { get; }

        public CardJson(
            CardDataJson cardData,
            int deckId,
            int currentDamage = 0,
            List<int> attachedEnergy = null,
            List<int> preEvolutionIds = null
        )
        {
            CardData = cardData;
            DeckId = deckId;
            CurrentDamage = currentDamage;
            AttachedEnergy = attachedEnergy ?? new List<int>();
            PreEvolutionIds = preEvolutionIds ?? new List<int>();
        }
    }
}
