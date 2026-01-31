using System;
using gamecore.serialization;

namespace gamecore.card
{
    public enum EnergyType
    {
        None,
        Grass,
        Fire,
        Water,
        Lightning,
        Fighting,
        Psychic,
        Colorless,
        Darkness,
        Metal,
        Dragon,
    }

    public static class EnergyTypeToProtoBufExtensions
    {
        public static ProtoBufEnergyType ToProtoBuf(this EnergyType energyType)
        {
            return energyType switch
            {
                EnergyType.None => ProtoBufEnergyType.EnergyTypeNone,
                EnergyType.Grass => ProtoBufEnergyType.EnergyTypeGrass,
                EnergyType.Fire => ProtoBufEnergyType.EnergyTypeFire,
                EnergyType.Water => ProtoBufEnergyType.EnergyTypeWater,
                EnergyType.Lightning => ProtoBufEnergyType.EnergyTypeLightning,
                EnergyType.Fighting => ProtoBufEnergyType.EnergyTypeFighting,
                EnergyType.Psychic => ProtoBufEnergyType.EnergyTypePsychic,
                EnergyType.Colorless => ProtoBufEnergyType.EnergyTypeColorless,
                EnergyType.Darkness => ProtoBufEnergyType.EnergyTypeDarkness,
                EnergyType.Metal => ProtoBufEnergyType.EnergyTypeMetal,
                EnergyType.Dragon => ProtoBufEnergyType.EnergyTypeDragon,
                _ => throw new InvalidOperationException($"Invalid energy type: {energyType}"),
            };
        }

        public static EnergyType FromProtoBuf(this ProtoBufEnergyType protoBufEnergyType)
        {
            return protoBufEnergyType switch
            {
                ProtoBufEnergyType.EnergyTypeNone => EnergyType.None,
                ProtoBufEnergyType.EnergyTypeGrass => EnergyType.Grass,
                ProtoBufEnergyType.EnergyTypeFire => EnergyType.Fire,
                ProtoBufEnergyType.EnergyTypeWater => EnergyType.Water,
                ProtoBufEnergyType.EnergyTypeLightning => EnergyType.Lightning,
                ProtoBufEnergyType.EnergyTypeFighting => EnergyType.Fighting,
                ProtoBufEnergyType.EnergyTypePsychic => EnergyType.Psychic,
                ProtoBufEnergyType.EnergyTypeColorless => EnergyType.Colorless,
                ProtoBufEnergyType.EnergyTypeDarkness => EnergyType.Darkness,
                ProtoBufEnergyType.EnergyTypeMetal => EnergyType.Metal,
                ProtoBufEnergyType.EnergyTypeDragon => EnergyType.Dragon,
                _ => throw new InvalidOperationException(
                    $"Invalid energy type: {protoBufEnergyType}"
                ),
            };
        }
    }
}
