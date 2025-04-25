using System;
using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.action
{
    internal class AttackGA : GameAction
    {
        public AttackGA(IAttackLogic attack, IPokemonCardLogic attacker)
        {
            Attack = attack;
            Attacker = attacker;
        }

        public IAttackLogic Attack { get; }
        public IPokemonCardLogic Attacker { get; }
    }
}
