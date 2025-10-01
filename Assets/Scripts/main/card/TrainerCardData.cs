using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using gamecore.instruction;

namespace gamecore.card
{
    internal interface ITrainerCardData : ICardData
    {
        List<IInstruction> Instructions { get; }
        List<IUseCondition> Conditions { get; }
    }

    abstract class TrainerCardData : ITrainerCardData
    {
        public string Name { get; }
        public string Id { get; }
        public List<IInstruction> Instructions { get; }
        public List<IUseCondition> Conditions { get; }

        protected TrainerCardData(
            string name,
            string id,
            List<IInstruction> instructions,
            List<IUseCondition> conditions
        )
        {
            Name = name;
            Id = id;
            Instructions = instructions;
            Conditions = conditions;
        }
    }
}
