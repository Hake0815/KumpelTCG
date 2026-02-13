using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class SetCardCurrentlyPlayedGA : GameAction
    {
        public ICardLogic Card { get; }

        [JsonConstructor]
        public SetCardCurrentlyPlayedGA(ICardLogic card)
        {
            Card = card;
        }
    }
}
