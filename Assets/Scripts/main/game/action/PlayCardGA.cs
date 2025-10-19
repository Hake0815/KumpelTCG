using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class PlayCardGA : GameAction
    {
        public ICardLogic Card { get; }
        public List<ICardLogic> Targets { get; } = new();

        [JsonConstructor]
        public PlayCardGA(ICardLogic card, List<ICardLogic> targets = null)
        {
            Card = card;
            if (targets != null)
                Targets.AddRange(targets);
        }
    }
}
