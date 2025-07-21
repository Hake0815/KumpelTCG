using System.Collections.Generic;
using gamecore.action;
using gamecore.actionsystem;
using gamecore.game;
using gamecore.instruction;
using gamecore.instruction.filter;

namespace gamecore.card
{
    static class CardDatabase
    {
        public static Dictionary<string, ICardData> cardDataDict = new()
        {
            {
                "professorsResearch",
                new SupporterCardData(
                    "Professor's Research",
                    "professorsResearch",
                    new List<IInstruction>
                    {
                        new DiscardInstruction(DiscardInstruction.TargetSource.Hand),
                        new DrawCardsInstruction(7),
                    },
                    new List<IUseCondition> { new HasCardsInDeck() }
                )
            },
            {
                "ultraBall",
                new ItemCardData(
                    name: "Ultra Ball",
                    id: "ultraBall",
                    instructions: new List<IInstruction>
                    {
                        new SelectCardsFromHandInstruction(
                            new IntRange(2, 2),
                            new ExcludeSourceCardNode()
                        ),
                        new DiscardInstruction(DiscardInstruction.TargetSource.Selection),
                        new SelectCardsFromDeckInstruction(
                            new IntRange(0, 1),
                            FilterUtils.CreatePokemonFilter()
                        ),
                        new TakeSelectionToHandInstruction(),
                        new DiscardInstruction(DiscardInstruction.TargetSource.Self),
                    },
                    conditions: new List<IUseCondition>
                    {
                        new HasAtLeastCardsInHand(3),
                        new HasCardsInDeck(),
                    }
                )
            },
            {
                "nightStretcher",
                new ItemCardData(
                    name: "Night Stretcher",
                    id: "nightStretcher",
                    instructions: new List<IInstruction>
                    {
                        new SelectCardsFromDiscardPileInstruction(
                            new IntRange(1, 1),
                            FilterUtils.CreatePokemonOrBasicEnergyFilter()
                        ),
                        new TakeSelectionToHandInstruction(),
                        new DiscardInstruction(DiscardInstruction.TargetSource.Self),
                    },
                    conditions: new List<IUseCondition>
                    {
                        new HasAtLeastCardsOfTypeInDiscardPile(
                            1,
                            card => card.IsPokemonCard() || card.IsBasicEnergyCard()
                        ),
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
                            new() { new DealDamageToDefendingPokemonInstruction(10) }
                        ),
                        new Attack(
                            "Bite",
                            new List<PokemonType> { PokemonType.Fire, PokemonType.Psychic },
                            new() { new DealDamageToDefendingPokemonInstruction(40) }
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
                            new() { new DealDamageToDefendingPokemonInstruction(70) }
                        ),
                    },
                    numberOfPrizeCardsOnKnockout: 1,
                    ability: new Ability(
                        "Recon Directive",
                        new List<IUseCondition> { new HasCardsInDeck(), new AbilityNotUsed() },
                        new List<IInstruction>
                        {
                            new RevealCardsFromDeckInstruction(2),
                            new SelectFromRevealedCardsInstruction(
                                new IntRange(1, 1),
                                new TrueNode()
                            ),
                            new TakeSelectionToHandInstruction(),
                            new PutRemainingCardsUnderDeckInstruction(),
                        }
                    )
                )
            },
            // EnergyCard Cards
            {
                "FireNRG",
                new BasicEnergyCardData(
                    id: "FireNRG",
                    name: "Fire Energy",
                    type: PokemonType.Fire,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "FightingNRG",
                new BasicEnergyCardData(
                    id: "FightingNRG",
                    name: "Fighting Energy",
                    type: PokemonType.Fighting,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "LightningNRG",
                new BasicEnergyCardData(
                    id: "LightningNRG",
                    name: "Lightning Energy",
                    type: PokemonType.Lightning,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "WaterNRG",
                new BasicEnergyCardData(
                    id: "WaterNRG",
                    name: "Water Energy",
                    type: PokemonType.Water,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "DarknessNRG",
                new BasicEnergyCardData(
                    id: "DarknessNRG",
                    name: "Darkness Energy",
                    type: PokemonType.Darkness,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "MetalNRG",
                new BasicEnergyCardData(
                    id: "MetalNRG",
                    name: "Metal Energy",
                    type: PokemonType.Metal,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "GrassNRG",
                new BasicEnergyCardData(
                    id: "GrassNRG",
                    name: "Grass Energy",
                    type: PokemonType.Grass,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "PsychicNRG",
                new BasicEnergyCardData(
                    id: "PsychicNRG",
                    name: "Psychic Energy",
                    type: PokemonType.Psychic,
                    energyCardType: EnergyCardType.Basic
                )
            },
        };
    }
}
