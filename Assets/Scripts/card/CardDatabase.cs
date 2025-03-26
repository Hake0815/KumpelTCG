using System.Collections.Generic;
using gamecore.effect;

namespace gamecore.card
{
    public class CardDatabase
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
        };
    }
}
