using System;
using System.Runtime.Serialization;
using gamecore.action;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.common;

namespace gamecore.effect
{
    internal class DealDamageToDefendingPokemonEffect : IEffect
    {
        public int Damage { get; }

        public DealDamageToDefendingPokemonEffect(int damage)
        {
            Damage = damage;
        }

        public void Perform(ICardLogic card)
        {
            if (!ActionSystem.INSTANCE.IsPerforming)
                throw new IlleagalStateException(
                    "Action system should be performing Attack game action."
                );
            ActionSystem.INSTANCE.AddReaction(
                new DealDamgeGA(
                    Damage,
                    card as IPokemonCardLogic,
                    card.Owner.Opponent.ActivePokemon
                )
            );
        }
    }
}
