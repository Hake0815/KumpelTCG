using System.Collections.Generic;
using gamecore.game.interaction;
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
                        new DiscardInstruction(TargetSource.Hand),
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
                            new FilterCondition(FilterType.ExcludeSource),
                            "discardSelection",
                            ActionOnSelection.Discard,
                            ActionOnSelection.Nothing
                        ),
                        new DiscardInstruction(TargetSource.Selection, "discardSelection"),
                        new SelectCardsFromDeckInstruction(
                            new IntRange(0, 1),
                            FilterUtils.CreatePokemonFilter(),
                            "deckSelection",
                            ActionOnSelection.TakeToHand,
                            ActionOnSelection.Nothing
                        ),
                        new ShowSelectedCardsInstruction("deckSelection"),
                        new TakeSelectionToHandInstruction("deckSelection"),
                        new ShuffleDeckInstruction(),
                        new DiscardInstruction(TargetSource.Self),
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
                            FilterUtils.CreatePokemonOrBasicEnergyFilter(),
                            "discardSelection",
                            ActionOnSelection.TakeToHand,
                            ActionOnSelection.Nothing
                        ),
                        new ShowSelectedCardsInstruction("discardSelection"),
                        new TakeSelectionToHandInstruction("discardSelection"),
                        new DiscardInstruction(TargetSource.Self),
                    },
                    conditions: new List<IUseCondition>
                    {
                        new HasAtLeastCardsOfTypeInDiscardPile(
                            1,
                            FilterUtils.CreatePokemonOrBasicEnergyFilter()
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
                    type: EnergyType.Dragon,
                    weakness: EnergyType.None,
                    resistance: EnergyType.None,
                    retreatCost: 1,
                    maxHP: 70,
                    attacks: new List<IAttackLogic>
                    {
                        new Attack(
                            "Petty Grudge",
                            new() { EnergyType.Psychic },
                            new() { new DealDamageToDefendingPokemonInstruction(10) }
                        ),
                        new Attack(
                            "Bite",
                            new List<EnergyType> { EnergyType.Fire, EnergyType.Psychic },
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
                    type: EnergyType.Dragon,
                    weakness: EnergyType.None,
                    resistance: EnergyType.None,
                    retreatCost: 1,
                    maxHP: 90,
                    attacks: new List<IAttackLogic>
                    {
                        new Attack(
                            "Dragon Headbutt",
                            new List<EnergyType> { EnergyType.Fire, EnergyType.Psychic },
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
                                new FilterCondition(FilterType.True),
                                "revealedSelection",
                                ActionOnSelection.TakeToHand,
                                ActionOnSelection.PutUnderDeck
                            ),
                            new TakeSelectionToHandInstruction("revealedSelection"),
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
                    type: EnergyType.Fire,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "FightingNRG",
                new BasicEnergyCardData(
                    id: "FightingNRG",
                    name: "Fighting Energy",
                    type: EnergyType.Fighting,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "LightningNRG",
                new BasicEnergyCardData(
                    id: "LightningNRG",
                    name: "Lightning Energy",
                    type: EnergyType.Lightning,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "WaterNRG",
                new BasicEnergyCardData(
                    id: "WaterNRG",
                    name: "Water Energy",
                    type: EnergyType.Water,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "DarknessNRG",
                new BasicEnergyCardData(
                    id: "DarknessNRG",
                    name: "Darkness Energy",
                    type: EnergyType.Darkness,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "MetalNRG",
                new BasicEnergyCardData(
                    id: "MetalNRG",
                    name: "Metal Energy",
                    type: EnergyType.Metal,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "GrassNRG",
                new BasicEnergyCardData(
                    id: "GrassNRG",
                    name: "Grass Energy",
                    type: EnergyType.Grass,
                    energyCardType: EnergyCardType.Basic
                )
            },
            {
                "PsychicNRG",
                new BasicEnergyCardData(
                    id: "PsychicNRG",
                    name: "Psychic Energy",
                    type: EnergyType.Psychic,
                    energyCardType: EnergyCardType.Basic
                )
            },
        };
    }
}
