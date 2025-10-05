using System.Collections.Generic;
using gamecore.action;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.common;

namespace gamecore.instruction
{
    class DealDamageToDefendingPokemonInstruction : IInstruction
    {
        public int Damage { get; }

        public DealDamageToDefendingPokemonInstruction(int damage)
        {
            Damage = damage;
        }

        public void Perform(ICardLogic card, ActionSystem actionSystem)
        {
            if (!actionSystem.IsPerforming)
                throw new IlleagalStateException(
                    "Action system should be performing Attack game action."
                );
            actionSystem.AddReaction(
                new DealDamgeGA(
                    Damage,
                    card as IPokemonCardLogic,
                    card.Owner.Opponent.ActivePokemon
                )
            );
        }

        public InstructionJson ToSerializable()
        {
            return new InstructionJson(
                instructionType: "deal_damage",
                data: new Dictionary<string, object>
                {
                    { "target", "defending" },
                    { "amount", Damage },
                }
            );
        }
    }
}
