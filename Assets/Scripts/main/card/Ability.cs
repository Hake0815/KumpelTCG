using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using gamecore.instruction;

namespace gamecore.card
{
    public interface IAbility
    {
        string Name { get; }
        AbilityJson ToSerializable();
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

        public AbilityJson ToSerializable()
        {
            // Convert instructions to InstructionJson objects
            var instructionJsons = new List<InstructionJson>();
            foreach (var instruction in Instructions)
            {
                instructionJsons.Add(instruction.ToSerializable());
            }

            // Convert conditions to ConditionJson objects
            var conditionJsons = new List<ConditionJson>();
            foreach (var condition in Conditions)
            {
                conditionJsons.Add(condition.ToSerializable());
            }

            return new AbilityJson(Name, instructionJsons, conditionJsons);
        }
    }
}
