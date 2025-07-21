using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

namespace gamecore.instruction
{
    class DiscardInstruction : IInstruction
    {
        public enum TargetSource
        {
            Hand,
            Self,
            Selection,
        }

        public TargetSource Target { get; }

        public DiscardInstruction(TargetSource target)
        {
            Target = target;
        }

        public void Perform(ICardLogic card)
        {
            List<ICardLogic> cardsToDiscard = new();
            switch (Target)
            {
                case TargetSource.Hand:
                    cardsToDiscard.AddRange(card.Owner.Hand.Cards);
                    ActionSystem.INSTANCE.AddReaction(
                        new RemoveCardsFromHandGA(new(cardsToDiscard), card.Owner)
                    );
                    ActionSystem.INSTANCE.AddReaction(new DiscardCardsGA(new(cardsToDiscard)));
                    break;
                case TargetSource.Self:
                    cardsToDiscard.Add(card);
                    ActionSystem.INSTANCE.AddReaction(new DiscardCardsGA(new(cardsToDiscard)));
                    break;
                case TargetSource.Selection:
                    new InstructionSubscriber<SelectCardsGA>(
                        action => Reaction(action),
                        ReactionTiming.POST
                    );
                    break;
            }
        }

        private static bool Reaction(SelectCardsGA action)
        {
            if (action.WasReactedTo)
                return false;

            ActionSystem.INSTANCE.AddReaction(new DiscardCardsGA(action.SelectedCards));
            action.WasReactedTo = true;
            return true;
        }

        public InstructionJson ToSerializable()
        {
            return new InstructionJson(
                instructionType: "discard",
                data: new Dictionary<string, object> { { "target", Target.ToString().ToLower() } }
            );
        }
    }
}
