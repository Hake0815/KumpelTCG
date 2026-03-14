using gamecore.card;
using gamecore.effect;
using gamecore.game;
using gamecore.serialization;

namespace gamecore.instruction
{
    class AbilityNotUsed : IUseCondition
    {
        public bool IsMet(ICardLogic card)
        {
            return !(card as IPokemonCardLogic).PokemonTurnTraits.Contains(
                PokemonTurnTrait.AbilityUsedThisTurn
            );
        }

        public ProtoBufCondition ToSerializable()
        {
            return new ProtoBufCondition
            {
                ConditionType = ProtoBufConditionType.ConditionTypeAbilityNotUsed,
            };
        }
    }
}
