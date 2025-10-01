using System;
using System.Collections.Generic;
using gamecore.instruction;

namespace gamecore.card
{
    [Serializable]
    public class AttackJson
    {
        public string Name { get; }
        public int Damage { get; }
        public List<EnergyType> EnergyCost { get; }
        public List<InstructionJson> Instructions { get; }

        public AttackJson(
            string name,
            int damage,
            List<EnergyType> energyCost = null,
            List<InstructionJson> instructions = null
        )
        {
            Name = name;
            Damage = damage;
            EnergyCost = energyCost ?? new List<EnergyType>();
            Instructions = instructions ?? new List<InstructionJson>();
        }
    }
}
