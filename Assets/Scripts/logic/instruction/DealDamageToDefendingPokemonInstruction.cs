using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.common;
using gamecore.game.action;
using gamecore.serialization;

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
                throw new IllegalStateException(
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

        public ProtoBufInstruction ToSerializable()
        {
            return new ProtoBufInstruction
            {
                InstructionType = ProtoBufInstructionType.InstructionTypeDealDamage,
                Data =
                {
                    new ProtoBufInstructionData
                    {
                        InstructionDataType =
                            ProtoBufInstructionDataType.InstructionDataTypeAttackData,
                        AttackData = new ProtoBufAttackInstructionData
                        {
                            AttackTarget = ProtoBufAttackTarget.AttackTargetDefendingPokemon,
                            Damage = Damage,
                        },
                    },
                },
            };
        }
    }
}
