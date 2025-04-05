using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using gamecore.effect;

namespace gamecore.card
{
    internal interface ITrainerCardData : ICardData
    {
        List<IEffect> Effects { get; }
        List<IPlayCondition> Conditions { get; }
    }

    internal class TrainerCardData : ITrainerCardData
    {
        public string Name { get; }
        public string Id { get; }
        public List<IEffect> Effects { get; }
        public List<IPlayCondition> Conditions { get; }

        public TrainerCardData(
            string name,
            string id,
            List<IEffect> effects,
            List<IPlayCondition> conditions
        )
        {
            Name = name;
            Id = id;
            Effects = effects;
            Conditions = conditions;
        }
    }
}
