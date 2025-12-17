using gamecore.actionsystem;
using gamecore.card;

namespace gamecore.game.action
{
    class AttackEndedGA : GameAction
    {
        public IPokemonCardLogic Attacker { get; }

        public AttackEndedGA(IPokemonCardLogic attacker)
        {
            Attacker = attacker;
            PostReactions.Add(new EndTurnGA());
        }
    }
}
