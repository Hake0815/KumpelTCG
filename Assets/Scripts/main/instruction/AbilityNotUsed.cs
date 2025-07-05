using gamecore.card;

namespace gamecore.instruction
{
    class AbilityNotUsed : IUseCondition
    {
        public bool IsMet(ICardLogic card)
        {
            return !(card as IPokemonCard).AbilityUsedThisTurn;
        }
    }
}
