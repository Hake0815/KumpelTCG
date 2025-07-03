using System.Collections.Generic;
using gamecore.action;
using gamecore.actionsystem;
using gamecore.effect;

namespace gamecore.card
{
    static class CardDatabase
    {
        public static Dictionary<string, ICardData> cardDataDict = new()
        {
            {
                "bill",
                new TrainerCardData(
                    "Bill",
                    "bill",
                    new List<IEffect> { new DrawCardsEffect(2), new DiscardCardEffect() },
                    new List<IUseCondition> { new HasCardsInDeck() }
                )
            },
            {
                "ultraBall",
                new TrainerCardData(
                    name: "Ultra Ball",
                    id: "ultraBall",
                    effects: new List<IEffect>
                    {
                        new SelectCardsFromHandEffect(2),
                        new DiscardSelectedCardsEffect(),
                        new SelectCardsFromDeckEffect(1, card => card.IsPokemonCard()),
                        new TakeSelectionToHandEffect(),
                        new DiscardCardEffect(),
                    },
                    conditions: new List<IUseCondition>
                    {
                        new HasAtLeatCardsInHand(3),
                        new HasCardsInDeck(),
                    }
                )
            },
            {
                "TWM128",
                new PokemonCardData(
                    id: "TWM128",
                    name: "Dreepy",
                    stage: Stage.Basic,
                    evolvesFrom: null,
                    type: PokemonType.Dragon,
                    weakness: PokemonType.None,
                    resistance: PokemonType.None,
                    retreatCost: 1,
                    maxHP: 70,
                    attacks: new List<IAttackLogic>
                    {
                        new Attack(
                            "Petty Grudge",
                            new() { PokemonType.Psychic },
                            new() { new DealDamageToDefendingPokemonEffect(10) }
                        ),
                        new Attack(
                            "Bite",
                            new List<PokemonType> { PokemonType.Fire, PokemonType.Psychic },
                            new() { new DealDamageToDefendingPokemonEffect(40) }
                        ),
                    },
                    numberOfPrizeCardsOnKnockout: 1
                )
            },
            {
                "TWM129",
                new PokemonCardData(
                    id: "TWM129",
                    name: "Drakloak",
                    stage: Stage.Stage1,
                    evolvesFrom: "Dreepy",
                    type: PokemonType.Dragon,
                    weakness: PokemonType.None,
                    resistance: PokemonType.None,
                    retreatCost: 1,
                    maxHP: 90,
                    attacks: new List<IAttackLogic>
                    {
                        new Attack(
                            "Dragon Headbutt",
                            new List<PokemonType> { PokemonType.Fire, PokemonType.Psychic },
                            new() { new DealDamageToDefendingPokemonEffect(70) }
                        ),
                    },
                    numberOfPrizeCardsOnKnockout: 1,
                    ability: new Ability(
                        "Recon Directive",
                        new List<IUseCondition> { new HasCardsInDeck(), new AbilityNotUsed() },
                        new List<IEffect>
                        {
                            new RevealCardsFromDeckEffect(2),
                            new SelectFromRevealedCardsEffect(1),
                            new TakeSelectionToHandEffect(),
                            new PutRemainingCardsUnderDeckEffect(),
                        }
                    )
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
