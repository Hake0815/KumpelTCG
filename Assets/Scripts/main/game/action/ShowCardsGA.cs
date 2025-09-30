using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class ShowCardsGA : GameAction
    {
        public List<ICardLogic> Cards { get; }

        [JsonConstructor]
        public ShowCardsGA(List<ICardLogic> cards)
        {
            Cards = cards;
        }
    }
}
