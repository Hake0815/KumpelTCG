using System.Collections.Generic;
using gamecore.action;
using gamecore.actionsystem;
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
                    new List<IAttackLogic>
                    {
                        new Attack(
                            "Petty Grudge",
                            new() { PokemonType.Psychic },
                            new() { new DealDamageToDefendingPokemonEffect(10) }
                        ),
                        new Attack(
                            " Bite",
                            new List<PokemonType> { PokemonType.Fire, PokemonType.Psychic },
                            new() { new DealDamageToDefendingPokemonEffect(40) }
                        ),
                    }
                )
            },
            // EnergyCard Cards
            {
                "FireNRG",
                new EnergyCardData(
                    id: "FireNRG",
                    name: "Fire Energy",
                    type: PokemonType.Fire,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "FightingNRG",
                new EnergyCardData(
                    id: "FightingNRG",
                    name: "Fighting Energy",
                    type: PokemonType.Fighting,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "LightningNRG",
                new EnergyCardData(
                    id: "LightningNRG",
                    name: "Lightning Energy",
                    type: PokemonType.Lightning,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "WaterNRG",
                new EnergyCardData(
                    id: "WaterNRG",
                    name: "Water Energy",
                    type: PokemonType.Water,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "DarknessNRG",
                new EnergyCardData(
                    id: "DarknessNRG",
                    name: "Darkness Energy",
                    type: PokemonType.Darkness,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "MetalNRG",
                new EnergyCardData(
                    id: "MetalNRG",
                    name: "Metal Energy",
                    type: PokemonType.Metal,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "GrassNRG",
                new EnergyCardData(
                    id: "GrassNRG",
                    name: "Grass Energy",
                    type: PokemonType.Grass,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "PsychicNRG",
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
