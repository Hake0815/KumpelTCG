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

        private readonly Dictionary<string, Sprite> _sprites = new()
        {
            { "bill", Resources.Load<Sprite>("Images/Cards/bill") },
            { "TWM128", Resources.Load<Sprite>("Images/Cards/TWM_128_R_EN_LG") },
            { "GrassNRG", Resources.Load<Sprite>("Images/Cards/Energy/SVE_001_R_EN_LG") },
            { "FireNRG", Resources.Load<Sprite>("Images/Cards/Energy/SVE_002_R_EN_LG") },
            { "WaterNRG", Resources.Load<Sprite>("Images/Cards/Energy/SVE_003_R_EN_LG") },
            { "LightningNRG", Resources.Load<Sprite>("Images/Cards/Energy/SVE_004_R_EN_LG") },
            { "PsychicNRG", Resources.Load<Sprite>("Images/Cards/Energy/SVE_005_R_EN_LG") },
            { "FightingNRG", Resources.Load<Sprite>("Images/Cards/Energy/SVE_006_R_EN_LG") },
            { "DarknessNRG", Resources.Load<Sprite>("Images/Cards/Energy/SVE_007_R_EN_LG") },
            { "MetalNRG", Resources.Load<Sprite>("Images/Cards/Energy/SVE_008_R_EN_LG") },
        };
        private readonly Dictionary<string, Sprite> _attachedSprites = new()
        {
            { "GrassNRG", Resources.Load<Sprite>("Images/Cards/Energy/SVE_001_R_EN_LG_attached") },
            { "FireNRG", Resources.Load<Sprite>("Images/Cards/Energy/SVE_002_R_EN_LG_attached") },
            { "WaterNRG", Resources.Load<Sprite>("Images/Cards/Energy/SVE_003_R_EN_LG_attached") },
            {
                "LightningNRG",
                Resources.Load<Sprite>("Images/Cards/Energy/SVE_004_R_EN_LG_attached")
            },
            {
                "PsychicNRG",
                Resources.Load<Sprite>("Images/Cards/Energy/SVE_005_R_EN_LG_attached")
            },
            {
                "FightingNRG",
                Resources.Load<Sprite>("Images/Cards/Energy/SVE_006_R_EN_LG_attached")
            },
            {
                "DarknessNRG",
                Resources.Load<Sprite>("Images/Cards/Energy/SVE_007_R_EN_LG_attached")
            },
            { "MetalNRG", Resources.Load<Sprite>("Images/Cards/Energy/SVE_008_R_EN_LG_attached") },
        };

        private readonly Sprite defaultSprite = Resources.Load<Sprite>("Images/Cards/default");

        public Sprite GetSprite(string id)
        {
            if (_sprites.TryGetValue(id, out var sprite))
            {
                return sprite;
            }
            Debug.LogError($"Sprite with id '{id}' not found in SpriteRegistry");
            return defaultSprite;
        }

        public Sprite GetAttachedSprite(string id)
        {
            if (_attachedSprites.TryGetValue(id, out var sprite))
            {
                return sprite;
            }
            return null;
        }
    }
}
