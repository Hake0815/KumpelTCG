using System;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class AttackGA : GameAction
    {
        [JsonConstructor]
        public AttackGA(IAttackLogic attack, IPokemonCardLogic attacker)
        {
            Attack = attack;
            Attacker = attacker;
            PostReactions.Add(CreateCheckKnockOutGA(attacker.Owner));
            PostReactions.Add(new EndTurnGA());
        }

        private static KnockOutCheckGA CreateCheckKnockOutGA(IPlayerLogic AttackingPlayer)
        {
            return new KnockOutCheckGA(new() { AttackingPlayer.Opponent, AttackingPlayer });
        }

        public IAttackLogic Attack { get; }
        public IPokemonCardLogic Attacker { get; }
    }
}
