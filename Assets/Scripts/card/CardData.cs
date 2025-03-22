using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using gamecore.effect;

namespace gamecore.card
{
    public abstract class CardData
    {
        public string Name { get; private set; }
        public List<IEffect> Effects { get; private set; }
        public List<IPlayCondition> Conditions { get; private set; }

        protected CardData(string name, List<IEffect> effects, List<IPlayCondition> conditions)
        {
            Name = name;
            Effects = effects;
            Conditions = conditions;
        }
    }

    public class CardDataDummy : CardData
    {
        public CardDataDummy(string name, List<IEffect> effects, List<IPlayCondition> conditions)
            : base(name, effects, conditions) { }
    }
}
