using System.Collections.Generic;

namespace gamecore.card
{
    public interface IAttack
    {
        string Name { get; }
        int Damage { get; }
        List<PokemonType> Cost { get; }
    }

    internal class Attack : IAttack
    {
        public string Name { get; }
        public int Damage { get; }
        public List<PokemonType> Cost { get; }

        public Attack(string name, int damage, List<PokemonType> cost)
        {
            Name = name;
            Damage = damage;
            Cost = cost;
        }
    }
}
