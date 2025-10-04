using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using gamecore.instruction.filter;

namespace gamecore.instruction
{
    class SelectFromRevealedCardsInstruction : SelectCardsInstruction
    {
        public SelectFromRevealedCardsInstruction(
            IntRange countRange,
            FilterNode filter,
            string selectionId
        )
            : base(countRange, filter, selectionId) { }

        public override void Perform(ICardLogic card, ActionSystem actionSystem)
        {
            new InstructionSubscriber<RevealCardsFromDeckGA>(
                action => Reaction(action, card, actionSystem),
                ReactionTiming.POST,
                actionSystem
            );
        }

        private bool Reaction(
            RevealCardsFromDeckGA action,
            ICardLogic card,
            ActionSystem actionSystem
        )
        {
            actionSystem.AddReaction(
                new QuickSelectCardsGA(
                    card.Owner,
                    CountRange.Contains,
                    new CardListLogic(action.RevealedCards),
                    c => Filter.Matches(c, card),
                    SelectCardsGA.SelectedCardsOrigin.Other,
                    SelectionId
                )
            );
            return true;
        }

        public override InstructionJson ToSerializable()
        {
            return new InstructionJson(
                instructionType: "select_cards",
                data: new Dictionary<string, object>
                {
                    { "from", "revealed" },
                    {
                        "count",
                        new Dictionary<string, object>
                        {
                            { "min", CountRange.Min },
                            { "max", CountRange.Max },
                        }
                    },
                    { "filter", Filter.ToSerializable() },
                }
            );
        }
    }
}
