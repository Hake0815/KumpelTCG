using System;
using System.Collections.Generic;
using gamecore.instruction;

namespace gamecore.card
{
    [Serializable]
    public class CardJson
    {
        // Static card data properties (moved from CardDataJson)
        public string Name { get; }
        public CardType CardType { get; }
        public CardSubtype CardSubtype { get; }
        public EnergyType EnergyType { get; }

        // Pokemon-specific static properties
        public int MaxHp { get; }
        public string EvolvesFrom { get; }
        public EnergyType Weakness { get; }
        public EnergyType Resistance { get; }
        public int RetreatCost { get; }
        public int NumberOfPrizeCardsOnKnockout { get; }
        public List<AttackJson> Attacks { get; }
        public AbilityJson Ability { get; }

        // Trainer-specific static properties
        public List<InstructionJson> Instructions { get; }
        public List<ConditionJson> Conditions { get; }

        // Current state properties
        public int DeckId { get; }

        // Pokemon-specific current state
        public int CurrentDamage { get; }
        public List<int> AttachedEnergy { get; }
        public List<int> PreEvolutionIds { get; }

        public CardJson(
            // Static card data parameters
            string name,
            CardType cardType,
            CardSubtype cardSubtype,
            EnergyType energyType,
            int maxHp = 0,
            string evolvesFrom = null,
            EnergyType weakness = EnergyType.None,
            EnergyType resistance = EnergyType.None,
            int retreatCost = 0,
            int numberOfPrizeCardsOnKnockout = 0,
            List<AttackJson> attacks = null,
            AbilityJson ability = null,
            List<InstructionJson> instructions = null,
            List<ConditionJson> conditions = null,
            // Current state parameters
            int deckId = 0,
            int currentDamage = 0,
            List<int> attachedEnergy = null,
            List<int> preEvolutionIds = null
        )
        {
            Name = name;
            CardType = cardType;
            CardSubtype = cardSubtype;
            EnergyType = energyType;
            MaxHp = maxHp;
            EvolvesFrom = evolvesFrom;
            Weakness = weakness;
            Resistance = resistance;
            RetreatCost = retreatCost;
            NumberOfPrizeCardsOnKnockout = numberOfPrizeCardsOnKnockout;
            Attacks = attacks ?? new List<AttackJson>();
            Ability = ability;
            Instructions = instructions ?? new List<InstructionJson>();
            Conditions = conditions ?? new List<ConditionJson>();

            // Current state
            DeckId = deckId;
            CurrentDamage = currentDamage;
            AttachedEnergy = attachedEnergy ?? new List<int>();
            PreEvolutionIds = preEvolutionIds ?? new List<int>();
        }
    }
}
