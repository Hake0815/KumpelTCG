using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using gamecore.effect;

namespace gamecore.card
{
    internal interface ITrainerCardData : ICardData
    {
        List<IEffect> Effects { get; }
        List<IUseCondition> Conditions { get; }
    }

    abstract class TrainerCardData : ITrainerCardData
    {
        public string Name { get; }
        public string Id { get; }
        public List<IEffect> Effects { get; }
        public List<IUseCondition> Conditions { get; }

        protected TrainerCardData(
            string name,
            string id,
            List<IEffect> effects,
            List<IUseCondition> conditions
        )
        {
            Name = name;
            Id = id;
            Effects = effects;
            Conditions = conditions;
        }
    }
}
