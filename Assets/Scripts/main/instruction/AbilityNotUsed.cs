using gamecore.card;
using gamecore.effect;
using gamecore.serialization;

namespace gamecore.instruction
{
    class AbilityNotUsed : IUseCondition
    {
        public bool IsMet(ICardLogic card)
        {
            return !(card as IPokemonCardLogic).HasEffect<AbilityUsedThisTurnEffect>();
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
