using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class RevealCardsFromDeckGA : GameAction
    {
        public IPlayerLogic Player { get; }
        public int Count { get; }
        public List<ICardLogic> RevealedCards { get; } = new();

        [JsonConstructor]
        public RevealCardsFromDeckGA(IPlayerLogic player, int count)
        {
            Player = player;
            Count = count;
        }
    }
}
