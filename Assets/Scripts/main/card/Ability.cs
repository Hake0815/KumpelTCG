using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using gamecore.instruction;

namespace gamecore.card
{
    public interface IAbility
    {
        string Name { get; }
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
    }
}
