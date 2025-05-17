using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.effect
{
    class RevealCardsFromDeckEffect : IEffect
    {
        public int Count { get; }

        public RevealCardsFromDeckEffect(int cardsToReveal)
        {
            Count = cardsToReveal;
        }

        public async Task Perform(ICardLogic card)
        {
            await ActionSystem.INSTANCE.Perform(new RevealCardsFromDeckGA(card.Owner, Count));
        }
    }
}
