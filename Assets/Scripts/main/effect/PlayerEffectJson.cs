using System;

namespace gamecore.effect
{
    [Serializable]
    public class PlayerEffectJson
    {
        public string TypeName { get; }

        public PlayerEffectJson(string typeName)
        {
            TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
        }
    }
}
