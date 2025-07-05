using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using UnityEngine;

namespace gamecore.instruction
{
    class RevealCardsFromDeckInstruction : IInstruction
    {
        public int Count { get; }

        public RevealCardsFromDeckInstruction(int cardsToReveal)
        {
            Count = cardsToReveal;
        }

        public void Perform(ICardLogic card)
        {
            ActionSystem.INSTANCE.AddReaction(new RevealCardsFromDeckGA(card.Owner, Count));
        }
    }
}
