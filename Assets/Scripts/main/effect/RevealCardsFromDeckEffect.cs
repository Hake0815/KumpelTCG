using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using UnityEngine;

namespace gamecore.effect
{
    class RevealCardsFromDeckEffect : IEffect
    {
        public int Count { get; }

        public RevealCardsFromDeckEffect(int cardsToReveal)
        {
            Count = cardsToReveal;
        }

        public void Perform(ICardLogic card)
        {
            ActionSystem.INSTANCE.AddReaction(new RevealCardsFromDeckGA(card.Owner, Count));
        }
    }
}
