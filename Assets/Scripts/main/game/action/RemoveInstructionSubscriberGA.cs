using System;
using gamecore.actionsystem;
using gamecore.instruction;

namespace gamecore.game.action
{
    class RemoveInstructionSubscriberGA<T> : GameAction
        where T : GameAction
    {
        public Guid Guid { get; }

        public RemoveInstructionSubscriberGA(Guid guid)
        {
            Guid = guid;
        }
    }
}
