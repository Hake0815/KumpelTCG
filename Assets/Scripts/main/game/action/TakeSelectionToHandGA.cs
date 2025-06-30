using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class TakeSelectionToHandGA : DoOnSelectionGA
    {
        [JsonConstructor]
        public TakeSelectionToHandGA(
            List<ICardLogic> selectedCards,
            IPlayerLogic player,
            List<ICardLogic> remainingCards
        )
            : base(selectedCards, player, remainingCards) { }
    }
}
