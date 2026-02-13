using System.Collections.Generic;
using gamecore.actionsystem;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    public class CreateGameGA : GameAction
    {
        [JsonConstructor]
        public CreateGameGA(
            Dictionary<string, int> deckList1,
            Dictionary<string, int> deckList2,
            string player1Name,
            string player2Name
        )
        {
            DeckList1 = deckList1;
            DeckList2 = deckList2;
            Player1Name = player1Name;
            Player2Name = player2Name;
            PostReactions.Add(new SetupGA());
        }

        public Dictionary<string, int> DeckList1 { get; }
        public Dictionary<string, int> DeckList2 { get; }
        public string Player1Name { get; }
        public string Player2Name { get; }
    }
}
