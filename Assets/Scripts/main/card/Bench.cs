using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using gamecore.actionsystem;
using gamecore.game.action;

namespace gamecore.card
{
    public interface IBench : ICardList
    {
        int MaxBenchSpots { get; }
        bool Full
        {
            get => CardCount >= MaxBenchSpots;
        }
    }

    internal interface IBenchLogic : IBench, ICardListLogic
    {
        new int MaxBenchSpots { get; set; }
        int IBench.MaxBenchSpots => MaxBenchSpots;
    }

    internal class Bench : IBenchLogic, IActionPerformer<BenchPokemonGA>
    {
        public List<ICardLogic> Cards { get; } = new();
        public int MaxBenchSpots { get; set; } = 5;

        public event EventHandler<List<ICard>> CardsAdded;
        public event Action CardsRemoved;

        public Bench()
        {
            ActionSystem.INSTANCE.AttachPerformer(this);
        }

        public void Clear()
        {
            var removedCards = ((ICardList)this).Cards;
            Cards.Clear();
            OnCardsRemoved(removedCards);
        }

        public IEnumerator GetEnumerator()
        {
            return Cards.GetEnumerator();
        }

        public void AddCards(List<ICardLogic> cards)
        {
            Cards.AddRange(cards);
            OnCardsAddedToBench(cards.Cast<ICard>().ToList());
        }

        public void RemoveCards(List<ICard> cards)
        {
            Cards.RemoveAll(cards.Contains);
            OnCardsRemoved(cards);
        }

        public void RemoveCard(ICard card)
        {
            Cards.Remove((ICardLogic)card);
            OnCardsRemoved(new() { card });
        }

        protected virtual void OnCardsRemoved(List<ICard> cards)
        {
            CardsRemoved?.Invoke();
        }

        protected virtual void OnCardsAddedToBench(List<ICard> cards)
        {
            CardsAdded?.Invoke(this, cards);
        }

        public BenchPokemonGA Perform(BenchPokemonGA action)
        {
            var pokemon = action.Card;
            pokemon.Owner.Bench.AddCards(new() { pokemon });
            pokemon.Owner.Hand.RemoveCard(pokemon);
            return action;
        }
    }
}
