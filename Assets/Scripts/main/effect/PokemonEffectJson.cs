using System;

namespace gamecore.effect
{
    [Serializable]
    public class PokemonEffectJson
    {
        public string Name { get; }

        public PokemonEffectJson(string name)
        {
            Name = name;
        }
    }
}
