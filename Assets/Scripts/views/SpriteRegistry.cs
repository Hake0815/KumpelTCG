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
        };

        public Sprite GetSprite(string id)
        {
            return sprites[id];
        }
    }
}
