using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using gamecore.serialization;

namespace gamecore.instruction
{
    class DiscardInstruction : IInstruction
    {
        public TargetSource Target { get; }

        private readonly string _selectionId;

        public DiscardInstruction(TargetSource target)
            : this(target, null) { }

        public DiscardInstruction(TargetSource target, string selectionId)
        {
            Target = target;
            _selectionId = selectionId;
        }

        public void Perform(ICardLogic card, ActionSystem actionSystem)
        {
            List<ICardLogic> cardsToDiscard = new();
            switch (Target)
            {
                case TargetSource.Hand:
                    cardsToDiscard.AddRange(card.Owner.Hand.Cards);
                    actionSystem.AddReaction(
                        new RemoveCardsFromHandGA(new(cardsToDiscard), card.Owner)
                    );
                    actionSystem.AddReaction(new DiscardCardsGA(new(cardsToDiscard)));
                    break;
                case TargetSource.Self:
                    cardsToDiscard.Add(card);
                    actionSystem.AddReaction(new DiscardCardsGA(new(cardsToDiscard)));
                    break;
                case TargetSource.Selection:
                    new InstructionSubscriber<SelectCardsGA>(
                        action => Reaction(action, actionSystem),
                        ReactionTiming.POST,
                        actionSystem
                    );
                    break;
            }
        }

        private bool Reaction(SelectCardsGA action, ActionSystem actionSystem)
        {
            if (action.SelectionId != _selectionId)
                return false;

            actionSystem.AddReaction(new DiscardCardsGA(action.SelectedCards));
            return true;
        }

        public InstructionJson ToSerializable()
        {
            return new InstructionJson(
                instructionType: InstructionType.Discard,
                data: new()
                {
                    new InstructionDataJson(
                        InstructionDataType.DiscardData,
                        new DiscardInstructionDataJson(Target)
                    ),
                }
            );
        }
    }

    public enum TargetSource
    {
        Hand,
        Self,
        Selection,
    }
}
