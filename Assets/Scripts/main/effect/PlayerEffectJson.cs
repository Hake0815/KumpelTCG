using System;
using gamecore.common;

namespace gamecore.effect
{
    [Serializable]
    public class PlayerEffectJson : JsonStringSerializable
    {
        public string TypeName { get; }

        public PlayerEffectJson(string typeName)
        {
            TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
        }
    }
}
