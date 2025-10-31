using System;
using gamecore.actionsystem;
using gamecore.instruction;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class RemoveInstructionSubscriberGA : GameAction
    {
        [JsonIgnore]
        public Action UnsubscribeAction { get; }

        public RemoveInstructionSubscriberGA(Action unsubscribeAction)
        {
            UnsubscribeAction = unsubscribeAction;
        }

        [JsonConstructor]
        public RemoveInstructionSubscriberGA() { }
    }
}
