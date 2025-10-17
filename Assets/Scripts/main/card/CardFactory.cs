using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using gamecore.common;
using gamecore.game;

namespace gamecore.card
{
    static class CardFactory
    {
        public static ICardLogic CreateCard(string id, IPlayerLogic owner, int deckId)
        {
            if (!CardDatabase.cardDataDict.ContainsKey(id))
            {
                GlobalLogger.Instance.Error(
                    $"ERROR: Card with ID '{id}' not found in CardDatabase"
                );
                return null;
            }

            var cardData = CardDatabase.cardDataDict[id];
            if (cardData is SupporterCardData supporterCardData)
            {
                return new SupporterCard(supporterCardData, owner, deckId);
            }
            if (cardData is ItemCardData itemCardData)
            {
                return new ItemCard(itemCardData, owner, deckId);
            }
            else if (cardData is IPokemonCardData pokemonCardData)
            {
                return new PokemonCard(pokemonCardData, owner, deckId);
            }
            else if (cardData is BasicEnergyCardData basicEnergyCardData)
            {
                return new BasicEnergyCard(basicEnergyCardData, owner, deckId);
            }
            GlobalLogger.Instance.Error(
                $"ERROR: Card data for ID '{id}' is neither a TrainerCardData nor a PokemonCardData"
            );
            return null;
        }

        public static List<ICardLogic> CreateCard(
            string id,
            IPlayerLogic owner,
            int count,
            int deckId
        )
        {
            return Enumerable
                .Range(0, count)
                .Select(i => CreateCard(id, owner, deckId + i))
                .ToList();
        }
    }
}
