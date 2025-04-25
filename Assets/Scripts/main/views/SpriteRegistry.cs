using System;
using System.Collections.Generic;
using gamecore.card;
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
        private readonly Dictionary<string, Sprite> _iconSprite = new()
        {
            { "GrassNRG", Resources.Load<Sprite>("Images/Icons/grass_icon") },
            { "FireNRG", Resources.Load<Sprite>("Images/Icons/fire_icon") },
            { "WaterNRG", Resources.Load<Sprite>("Images/Icons/water_icon") },
            { "LightningNRG", Resources.Load<Sprite>("Images/Icons/lightning_icon") },
            { "PsychicNRG", Resources.Load<Sprite>("Images/Icons/psychic_icon") },
            { "FightingNRG", Resources.Load<Sprite>("Images/Icons/fighting_icon") },
            { "DarknessNRG", Resources.Load<Sprite>("Images/Icons/darkness_icon") },
            { "MetalNRG", Resources.Load<Sprite>("Images/Icons/metal_icon") },
            { "ColorlessNRG", Resources.Load<Sprite>("Images/Icons/colorless_icon") },
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

        public Sprite GetIconSprite(string id)
        {
            if (_iconSprite.TryGetValue(id, out var sprite))
            {
                return sprite;
            }
            return null;
        }

        public Sprite GetTypeIcon(PokemonType type)
        {
            return type switch
            {
                PokemonType.Grass => _iconSprite["GrassNRG"],
                PokemonType.Fire => _iconSprite["FireNRG"],
                PokemonType.Water => _iconSprite["WaterNRG"],
                PokemonType.Lightning => _iconSprite["LightningNRG"],
                PokemonType.Psychic => _iconSprite["PsychicNRG"],
                PokemonType.Fighting => _iconSprite["FightingNRG"],
                PokemonType.Darkness => _iconSprite["DarknessNRG"],
                PokemonType.Metal => _iconSprite["MetalNRG"],
                PokemonType.Colorless => _iconSprite["ColorlessNRG"],
                _ => null,
            };
        }
    }
}
