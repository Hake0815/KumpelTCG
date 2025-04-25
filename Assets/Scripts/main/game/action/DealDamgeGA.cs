using System;
using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.action
{
    internal class DealDamgeGA : GameAction
    {
        public int Damage { get; set; }
        public IPokemonCardLogic Attacker { get; }
        public IPokemonCardLogic Target { get; }
        public int ModifierBeforeWeaknessResistance { get; set; } = 0;
        public int ModifierAfterWeaknessResistance { get; set; } = 0;
        public bool IsNegated { get; set; } = false;

        public DealDamgeGA(int damage, IPokemonCardLogic attacker, IPokemonCardLogic target)
        {
            Damage = damage;
            Attacker = attacker;
            Target = target;
        }
    }
}
