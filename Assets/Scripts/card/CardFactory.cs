using gamecore.game;

namespace gamecore.card
{
    public class CardFactory
    {
        public static ICard CreateCard(string id, IPlayer owner)
        {
            var cardData = CardDatabase.cardDataDict[id];
            if (cardData is ITrainerCardData)
            {
                return new TrainerCard(cardData as ITrainerCardData, owner);
            }
            else if (cardData is IPokemonCardData)
            {
                return new PokemonCard(cardData as IPokemonCardData, owner);
            }
            return null;
        }
    }
}
