using System;
using System.Collections;
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
        event Action CardCountChanged;

        IEnumerator<ICard> IEnumerable<ICard>.GetEnumerator()
        {
            return Cards.GetEnumerator();
        }
    }

    internal interface ICardListLogic : ICardList, IEnumerable<ICardLogic>
    {
        new List<ICardLogic> Cards { get; }
        List<ICard> ICardList.Cards => Cards.Cast<ICard>().ToList();
        protected static readonly Random rng = new();

        void Shuffle()
        {
            var n = CardCount;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (Cards[n], Cards[k]) = (Cards[k], Cards[n]);
            }
        }
        void AddCards(List<ICardLogic> cards)
        {
            Cards.AddRange(cards);
            OnCardCountChanged();
        }

        void RemoveCards(List<ICard> cards)
        {
            Cards.RemoveAll(cards.Contains);
            OnCardCountChanged();
        }

        void RemoveCard(ICard card)
        {
            Cards.Remove((ICardLogic)card);
            OnCardCountChanged();
        }

        void Clear()
        {
            Cards.Clear();
            OnCardCountChanged();
        }
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Cards.GetEnumerator();
        }

        void OnCardCountChanged();
    }
}
