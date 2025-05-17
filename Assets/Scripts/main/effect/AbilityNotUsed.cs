using gamecore.card;

namespace gamecore.effect
{
    class AbilityNotUsed : IUseCondition
    {
        public bool IsMet(ICardLogic card)
        {
            return !(card as IPokemonCard).AbilityUsedThisTurn;
        }
    }
}
