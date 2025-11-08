using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace gamecore.serialization
{
    [Serializable]
    public class GameStateJson : IJsonStringSerializable
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
