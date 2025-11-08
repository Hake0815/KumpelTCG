using System;
using System.Collections.Generic;
using gamecore.card;
using gamecore.instruction;

namespace gamecore.serialization
{
    [Serializable]
    public class AttackJson : IJsonStringSerializable
    {
        public string Name { get; }
        public int Damage { get; }
        public List<EnergyType> EnergyCost { get; }
        public List<InstructionJson> Instructions { get; }

        public AttackJson(
            string name,
            int damage,
            List<EnergyType> energyCost,
            List<InstructionJson> instructions
        )
        {
            Name = name;
            Damage = damage;
            EnergyCost = energyCost;
            Instructions = instructions;
        }
    }
}
