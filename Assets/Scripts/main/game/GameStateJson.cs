using System;
using System.Collections.Generic;

namespace gamecore.game
{
    [Serializable]
    public class GameStateJson
    {
        public PlayerStateJson SelfState { get; }
        public PlayerStateJson OpponentState { get; }
        public List<CardStateJson> CardStates { get; }

        public GameStateJson(
            PlayerStateJson selfState,
            PlayerStateJson opponentState,
            List<CardStateJson> cardStates
        )
        {
            SelfState = selfState ?? throw new ArgumentNullException(nameof(selfState));
            OpponentState = opponentState ?? throw new ArgumentNullException(nameof(opponentState));
            CardStates = cardStates ?? new List<CardStateJson>();
        }
    }
}
