using System;
using System.Collections.Generic;

namespace gamecore.serialization
{
    [Serializable]
    public class PositionJson : IJsonStringSerializable
    {
        public Owner Owner { get; }
        public List<CardPosition> PossiblePositions { get; }
        public int? TopDeckPositionIndex { get; }
        public int? AttachedToPokemonId { get; }

        public PositionJson(
            Owner owner,
            List<CardPosition> possiblePositions,
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
