using System;
using gamecore.actionsystem;

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
