using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class DoOnSelectionGA : GameAction
    {
        [JsonConstructor]
        public DoOnSelectionGA(
            List<ICardLogic> selectedCards,
            IPlayerLogic player,
            List<ICardLogic> remainingCards
        )
        {
            SelectedCards = new(selectedCards);
            Player = player;
            RemainingCards = new(remainingCards);
        }

        public List<ICardLogic> SelectedCards { get; }
        public IPlayerLogic Player { get; }
        public List<ICardLogic> RemainingCards { get; }
    }
}
