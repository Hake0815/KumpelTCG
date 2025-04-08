using System;
using System.Collections.Generic;
using UnityEngine;

namespace gameview
{
    public class SpriteRegistry
    {
        private static readonly Lazy<SpriteRegistry> lazy = new(() => new SpriteRegistry());
        public static SpriteRegistry INSTANCE => lazy.Value;

        private SpriteRegistry() { }

        private readonly Dictionary<string, Sprite> sprites = new()
        {
            { "bill", Resources.Load<Sprite>("Images/Cards/bill") },
            { "TWM128", Resources.Load<Sprite>("Images/Cards/TWM_128_R_EN_LG") },
        };

        private readonly Sprite defaultSprite = Resources.Load<Sprite>("Images/Cards/default");

        public Sprite GetSprite(string id)
        {
            if (sprites.TryGetValue(id, out var sprite))
            {
                return sprite;
            }
            Debug.LogError($"Sprite with id '{id}' not found in SpriteRegistry");
            return defaultSprite;
        }
    }
}
