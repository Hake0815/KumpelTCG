using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.instruction
{
    class SelectFromRevealedCardsInstruction : IInstruction
    {
        public int Amount { get; }

        public SelectFromRevealedCardsInstruction(int amount)
        {
            Amount = amount;
        }

        public void Perform(ICardLogic card)
        {
            new InstructionSubscriber<RevealCardsFromDeckGA>(
                action => Reaction(action, card),
                ReactionTiming.POST
            );
        }

        private bool Reaction(RevealCardsFromDeckGA action, ICardLogic card)
        {
            ActionSystem.INSTANCE.AddReaction(
                new QuickSelectCardsGA(
                    card.Owner,
                    Amount,
                    new CardListLogic(action.RevealedCards),
                    SelectCardsGA.SelectedCardsOrigin.Other
                )
            );
            return true;
        }
    }
}
