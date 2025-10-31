using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.interaction;
using gamecore.instruction.filter;

namespace gamecore.instruction
{
    abstract class SelectCardsInstruction : IInstruction
    {
        protected SelectCardsInstruction(
            IntRange countRange,
            FilterNode filter,
            string selectionId,
            ActionOnSelection targetAction,
            ActionOnSelection remainderAction
        )
        {
            CountRange = countRange;
            Filter = filter;
            SelectionId = selectionId;
            TargetAction = targetAction;
            RemainderAction = remainderAction;
        }

        public IntRange CountRange { get; }
        public FilterNode Filter { get; }
        public string SelectionId { get; }
        public ActionOnSelection TargetAction { get; }
        public ActionOnSelection RemainderAction { get; }
        public abstract void Perform(ICardLogic card, ActionSystem actionSystem);

        public abstract InstructionJson ToSerializable();
    }
}
