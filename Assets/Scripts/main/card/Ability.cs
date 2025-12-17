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
            return new ProtoBufAbility
            {
                Name = Name,
                Instructions = { Instructions.Select(instruction => instruction.ToSerializable()) },
                Conditions = { Conditions.Select(condition => condition.ToSerializable()) },
            };
        }
    }
}
