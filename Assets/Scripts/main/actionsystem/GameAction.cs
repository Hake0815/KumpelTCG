using System.Collections.Generic;
using Newtonsoft.Json;

namespace gamecore.actionsystem
{
    public abstract class GameAction
    {
        [JsonIgnore]
        public List<GameAction> PreReactions { get; private set; } = new();

        [JsonIgnore]
        public List<GameAction> PerformReactions { get; private set; } = new();

        [JsonIgnore]
        public List<GameAction> PostReactions { get; private set; } = new();
    }
}
