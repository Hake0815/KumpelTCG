using System;
using System.Collections.Generic;
using System.Linq;

namespace gamecore.card
{
    public interface ICardList : IEnumerable<ICard>
    {
        List<ICard> Cards { get; }
        int CardCount
        {
            get => Cards.Count;
        }
        event EventHandler<List<ICard>> CardsAdded;
        event Action CardsRemoved;

        IEnumerator<ICard> IEnumerable<ICard>.GetEnumerator()
        {
            return Cards.GetEnumerator();
        }
    }

    internal interface ICardListLogic : ICardList, IEnumerable<ICardLogic>
    {
        new List<ICardLogic> Cards { get; }
        List<ICard> ICardList.Cards => Cards.Cast<ICard>().ToList();
        void AddCards(List<ICardLogic> cards);
        void RemoveCards(List<ICard> cards);
        void RemoveCard(ICard card);
        void Clear();
        List<ICardLogic> GetBasicPokemon()
        {
            var basicPokemon = new List<ICardLogic>();
            foreach (var card in Cards)
            {
                if (card is IPokemonCard pokemonCard && pokemonCard.Stage == Stage.Basic)
                {
                    basicPokemon.Add(card);
                }
            }
            return basicPokemon;
        }

        IEnumerator<ICardLogic> IEnumerable<ICardLogic>.GetEnumerator()
        {
            return Cards.GetEnumerator();
        }
    }
}
