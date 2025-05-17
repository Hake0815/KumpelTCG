using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using gamecore.effect;

namespace gamecore.card
{
    public interface IAbility
    {
        string Name { get; }
    }

    internal interface IAbilityLogic : IAbility
    {
        List<IEffect> Effects { get; }
        List<IUseCondition> Conditions { get; }
    }

    class Ability : IAbilityLogic
    {
        public List<IUseCondition> Conditions { get; }
        public List<IEffect> Effects { get; }

        public string Name { get; }

        public Ability(string name, List<IUseCondition> conditions, List<IEffect> effects)
        {
            Name = name;
            Conditions = conditions;
            Effects = effects;
        }
    }
}
