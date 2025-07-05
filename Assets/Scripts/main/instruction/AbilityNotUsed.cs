using gamecore.card;
using gamecore.effect;

namespace gamecore.instruction
{
    class AbilityNotUsed : IUseCondition
    {
        public bool IsMet(ICardLogic card)
        {
            return !(card as IPokemonCardLogic).HasEffect<AbilityUsedThisTurnEffect>();
        }
    }
}
