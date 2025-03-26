using gamecore.game;
using UnityEngine;

namespace gamecore.card
{
    public class CardFactory
    {
        public static ICard CreateCard(string id, IPlayer owner)
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
    }
}
