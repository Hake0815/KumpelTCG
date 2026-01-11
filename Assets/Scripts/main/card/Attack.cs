using System;
using System.Collections.Generic;
using System.Linq;
using gamecore.actionsystem;
using gamecore.common;
using gamecore.instruction;
using gamecore.serialization;

namespace gamecore.card
{
    public interface IAttack
    {
        string Name { get; }
        int Damage { get; }
        List<EnergyType> Cost { get; }
        ProtoBufAttack ToSerializable();
    }

    internal interface IAttackLogic : IAttack, IClonable<IAttackLogic>
    {
        List<IInstruction> Instructions { get; }
    }

    class Attack : IAttackLogic
    {
        public string Name { get; }
        public int Damage => GetDamageToActivePokemon();
        public List<EnergyType> Cost { get; }

        private int GetDamageToActivePokemon()
        {
            foreach (var instruction in Instructions)
            {
                if (
                    instruction
                        .GetType()
                        .IsAssignableFrom(typeof(DealDamageToDefendingPokemonInstruction))
                )
                {
                    return (instruction as DealDamageToDefendingPokemonInstruction).Damage;
                }
            }
            return 0;
        }

        public List<IInstruction> Instructions { get; }

        public Attack(string name, List<EnergyType> cost, List<IInstruction> instructions)
        {
            Name = name;
            Cost = cost;
            Instructions = instructions;
        }

        public IAttackLogic Clone()
        {
            return new Attack(Name, new(Cost), new(Instructions));
        }

        public ProtoBufAttack ToSerializable()
        {
            var protoBufAttack = new ProtoBufAttack { Name = Name, Damage = Damage };
            protoBufAttack.EnergyCost.Capacity = Cost.Count;
            foreach (var energyType in Cost)
            {
                protoBufAttack.EnergyCost.Add(energyType.ToProtoBuf());
            }
            protoBufAttack.Instructions.Capacity = Instructions.Count;
            foreach (var instruction in Instructions)
            {
                protoBufAttack.Instructions.Add(instruction.ToSerializable());
            }
            return protoBufAttack;
        }
    }
}
