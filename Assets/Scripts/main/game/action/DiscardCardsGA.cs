using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class DiscardCardsGA : GameAction
    {
        [JsonConstructor]
        public DiscardCardsGA(List<ICardLogic> cards)
        {
            Cards = cards;
        }

        public List<ICardLogic> Cards { get; }
    }
}
