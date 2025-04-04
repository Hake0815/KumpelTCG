using System.Collections.Generic;
using System.Linq;
using gamecore.game;
using UnityEngine;

namespace gamecore.card
{
    internal class CardFactory
    {
        internal static ICard CreateCard(string id, IPlayerLogic owner)
        {
            if (!CardDatabase.cardDataDict.ContainsKey(id))
            {
                Debug.LogError($"Card with ID '{id}' not found in CardDatabase");
                return null;
            }

            var cardData = CardDatabase.cardDataDict[id];
            if (cardData is ITrainerCardData)
            {
                return new TrainerCard(cardData as ITrainerCardData, owner);
            }
            else if (cardData is IPokemonCardData)
            {
                return new PokemonCard(cardData as IPokemonCardData, owner);
            }
            Debug.LogError(
                $"Card data for ID '{id}' is neither a TrainerCardData nor a PokemonCardData"
            );
            return null;
        }

        internal static List<ICard> CreateCard(string id, IPlayerLogic owner, int count)
        {
            return Enumerable.Range(0, count).Select(i => CreateCard(id, owner)).ToList();
        }
    }
}
