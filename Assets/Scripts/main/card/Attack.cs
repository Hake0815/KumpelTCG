using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.common;
using gamecore.effect;

namespace gamecore.card
{
    public interface IAttack
    {
        string Name { get; }
        int Damage { get; }
        List<PokemonType> Cost { get; }
    }

    internal interface IAttackLogic : IAttack, IClonable<IAttackLogic>
    {
        List<IEffect> Effects { get; }
    }

    class Attack : IAttackLogic
    {
        public string Name { get; }
        public int Damage => GetDamageToActivePokemon();
        public List<PokemonType> Cost { get; }

        private int GetDamageToActivePokemon()
        {
            foreach (var effect in Effects)
            {
                if (effect.GetType().IsAssignableFrom(typeof(DealDamageToDefendingPokemonEffect)))
                {
                    return (effect as DealDamageToDefendingPokemonEffect).Damage;
                }
            }
            return 0;
        }

        public List<IEffect> Effects { get; }

        public Attack(string name, List<PokemonType> cost, List<IEffect> effects)
        {
            Name = name;
            Cost = cost;
            Effects = effects;
        }

        public IAttackLogic Clone()
        {
            return new Attack(Name, new(Cost), new(Effects));
        }
    }
}
