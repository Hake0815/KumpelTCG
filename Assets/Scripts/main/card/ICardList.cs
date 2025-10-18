using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace gamecore.card
{
    public interface ICardList : IEnumerable<ICard>
    {
        List<ICard> Cards { get; }
        int CardCount { get; }
        event Action<List<ICard>> CardCountChanged;

        IEnumerator<ICard> IEnumerable<ICard>.GetEnumerator()
        {
            return Cards.GetEnumerator();
        }
    }

    internal abstract class CardListLogicAbstract : ICardList, IEnumerable<ICardLogic>
    {
        protected CardListLogicAbstract(List<ICardLogic> cards)
        {
            Cards = cards;
        }

        public List<ICardLogic> Cards { get; }
        public int CardCount
        {
            get => Cards.Count;
        }
        List<ICard> ICardList.Cards => Cards.Cast<ICard>().ToList();
        protected static readonly Random rng = new();

        public abstract event Action<List<ICard>> CardCountChanged;

        public virtual void Shuffle()
        {
            var n = CardCount;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (Cards[n], Cards[k]) = (Cards[k], Cards[n]);
            }
        }

        public virtual void AddCards(List<ICardLogic> cards)
        {
            Cards.AddRange(cards);
            OnCardCountChanged();
        }

        public virtual void RemoveCards(List<ICardLogic> cards)
        {
            Cards.RemoveAll(cards.Contains);
            OnCardCountChanged();
        }

        public virtual void RemoveCard(ICardLogic card)
        {
            Cards.Remove(card);
            OnCardCountChanged();
        }

        public virtual void Clear()
        {
            Cards.Clear();
            OnCardCountChanged();
        }

        public List<IPokemonCardLogic> GetBasicPokemon()
        {
            var basicPokemon = new List<IPokemonCardLogic>();
            foreach (var card in Cards)
            {
                if (card is IPokemonCardLogic pokemonCard && pokemonCard.Stage == Stage.Basic)
                {
                    basicPokemon.Add(pokemonCard);
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

        public abstract void OnCardCountChanged();
    }

    class CardListLogic : CardListLogicAbstract
    {
        public CardListLogic(List<ICardLogic> cards)
            : base(new(cards)) { }

        public override event Action<List<ICard>> CardCountChanged;

        public override void OnCardCountChanged() { }
    }
}
