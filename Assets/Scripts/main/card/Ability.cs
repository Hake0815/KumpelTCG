using System.Collections.Generic;
using System.Linq;
using gamecore.instruction;
using gamecore.serialization;

namespace gamecore.card
{
    public interface IAbility
    {
        string Name { get; }
        ProtoBufAbility ToSerializable();
    }

    internal interface IAbilityLogic : IAbility
    {
        List<IInstruction> Instructions { get; }
        List<IUseCondition> Conditions { get; }
    }

    class Ability : IAbilityLogic
    {
        public List<IUseCondition> Conditions { get; }
        public List<IInstruction> Instructions { get; }

        public string Name { get; }

        public Ability(string name, List<IUseCondition> conditions, List<IInstruction> instructions)
        {
            Name = name;
            Conditions = conditions;
            Instructions = instructions;
        }

        public ProtoBufAbility ToSerializable()
        {
            var protoBufAbility = new ProtoBufAbility { Name = Name };
            protoBufAbility.Instructions.Capacity = Instructions.Count;
            protoBufAbility.Conditions.Capacity = Conditions.Count;
            foreach (var instruction in Instructions)
            {
                protoBufAbility.Instructions.Add(instruction.ToSerializable());
            }
            foreach (var condition in Conditions)
            {
                protoBufAbility.Conditions.Add(condition.ToSerializable());
            }
            return protoBufAbility;
        }
    }
}
