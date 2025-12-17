using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using gamecore.game.interaction;
using gamecore.instruction.filter;
using gamecore.serialization;

namespace gamecore.instruction
{
    class SelectFromRevealedCardsInstruction : SelectCardsInstruction
    {
        public SelectFromRevealedCardsInstruction(
            IntRange countRange,
            FilterNode filter,
            string selectionId,
            ActionOnSelection targetAction,
            ActionOnSelection remainderAction
        )
            : base(countRange, filter, selectionId, targetAction, remainderAction) { }

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
                    new ConditionalTargetQuery(
                        new NumberRange(CountRange.Min, CountRange.Max),
                        SelectionQualifier.NumberOfCards
                    ),
                    action.RevealedCards,
                    c => Filter.Matches(c, card),
                    SelectCardsGA.SelectedCardsOrigin.Floating,
                    SelectionId,
                    TargetAction,
                    RemainderAction
                )
            );
            return true;
        }

        public override InstructionJson ToSerializable()
        {
            return new InstructionJson(
                instructionType: InstructionType.SelectCards,
                data: new()
                {
                    new InstructionDataJson(
                        InstructionDataType.CardAmountData,
                        new CardAmountInstructionDataJson(CountRange, CardPosition.Floating)
                    ),
                    new InstructionDataJson(
                        InstructionDataType.FilterData,
                        new FilterInstructionDataJson(Filter.ToSerializable())
                    ),
                }
            );
        }
    }
}
