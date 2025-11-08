using System;
using gamecore.common;

namespace gamecore.effect
{
    [Serializable]
    public class PokemonEffectJson : JsonStringSerializable
    {
        public string Name { get; }

        public PokemonEffectJson(string name)
        {
            Name = name;
        }
    }
}
