using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.common;
using gamecore.game.action;

namespace gamecore.instruction
{
    class InstructionSubscriberRemover : IActionPerformer<RemoveInstructionSubscriberGA>
    {
        public Task<RemoveInstructionSubscriberGA> Perform(RemoveInstructionSubscriberGA action)
        {
            action.UnsubscribeAction();
            return Task.FromResult(action);
        }

        public Task<RemoveInstructionSubscriberGA> Reperform(RemoveInstructionSubscriberGA action)
        {
            throw new IllegalStateException(
                "RemoveInstructionSubscriberGA should never be reperformed!"
            );
        }
    }
}
