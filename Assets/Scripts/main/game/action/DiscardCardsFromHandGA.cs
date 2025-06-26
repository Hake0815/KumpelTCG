using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class DiscardCardsFromHandGA : GameAction
    {
        public List<ICardLogic> Cards { get; }

        [JsonConstructor]
        public DiscardCardsFromHandGA(List<ICardLogic> cards)
        {
            Cards = cards;
        }
    }
}
