using System.Collections.Generic;
using gamecore.effect;

namespace gamecore.card
{
    public abstract class CardData
    {
        public string Name { get; private set; }
        public List<IEffect> Effects { get; private set; }

        protected CardData(string name, List<IEffect> effects)
        {
            Name = name;
            Effects = effects;
        }
    }

    public class CardDataDummy : CardData
    {
        public CardDataDummy(string name, List<IEffect> effects)
            : base(name, effects) { }
    }
}
