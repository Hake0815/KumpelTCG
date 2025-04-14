using System.Collections.Generic;
using gamecore.effect;

namespace gamecore.card
{
    internal class CardDatabase
    {
        public static Dictionary<string, ICardData> cardDataDict = new()
        {
            {
                "bill",
                new TrainerCardData(
                    "Bill",
                    "bill",
                    new List<IEffect> { new DrawCardsEffect(2), new DiscardCardEffect() },
                    new List<IPlayCondition> { new HasCardsInDeck() }
                )
            },
            {
                "TWM128",
                new PokemonCardData(
                    id: "TWM128",
                    name: "Dreepy",
                    stage: Stage.Basic,
                    type: PokemonType.Dragon,
                    weakness: PokemonType.None,
                    resistance: PokemonType.None,
                    retreatCost: 1,
                    maxHP: 70,
                    new List<IAttack>
                    {
                        new Attack(
                            "Petty Grudge",
                            10,
                            new List<PokemonType> { PokemonType.Psychic }
                        ),
                        new Attack(
                            " Bite",
                            40,
                            new List<PokemonType> { PokemonType.Fire, PokemonType.Psychic }
                        ),
                    }
                )
            },
            // EnergyCard Cards
            {
                "Fire Energy",
                new EnergyCardData(
                    id: "FireNRG",
                    name: "Fire Energy",
                    type: PokemonType.Fire,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "Fighting Energy",
                new EnergyCardData(
                    id: "FightingNRG",
                    name: "Fighting Energy",
                    type: PokemonType.Fighting,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "Lightning Energy",
                new EnergyCardData(
                    id: "LightningNRG",
                    name: "Lightning Energy",
                    type: PokemonType.Lightning,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "Water Energy",
                new EnergyCardData(
                    id: "WaterNRG",
                    name: "Water Energy",
                    type: PokemonType.Water,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "Darkness Energy",
                new EnergyCardData(
                    id: "DarknessNRG",
                    name: "Darkness Energy",
                    type: PokemonType.Darkness,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "Metal Energy",
                new EnergyCardData(
                    id: "MetalNRG",
                    name: "Metal Energy",
                    type: PokemonType.Metal,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "Grass Energy",
                new EnergyCardData(
                    id: "GrassNRG",
                    name: "Grass Energy",
                    type: PokemonType.Grass,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "Psychic Energy",
                new EnergyCardData(
                    id: "PsychicNRG",
                    name: "Psychic Energy",
                    type: PokemonType.Psychic,
                    energyCardType: EnergyCardType.Basic
                )
            },
        };
    }
}
