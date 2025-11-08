using System;
using System.Collections.Generic;
using gamecore.common;

namespace gamecore.game
{
    [Serializable]
    public class PositionJson : JsonStringSerializable
    {
        public Owner Owner { get; }
        public List<string> PossiblePositions { get; }
        public int? TopDeckPositionIndex { get; }
        public int? AttachedToPokemonId { get; }

        public PositionJson(
            Owner owner,
            List<string> possiblePositions = null,
            int? topDeckPositionIndex = null,
            int? attachedToPokemonId = null
        )
        {
            Owner = owner;
            PossiblePositions = possiblePositions;
            TopDeckPositionIndex = topDeckPositionIndex;
            AttachedToPokemonId = attachedToPokemonId;
        }
    }

    public enum Owner
    {
        Self,
        Opponent,
    }
}
