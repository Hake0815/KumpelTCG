using System;
using System.Collections.Generic;

namespace gamecore.card
{
    public interface IBench : ICardList
    {
        int MaxBenchSpots { get; }
        bool Full { get; }
    }

    internal abstract class BenchLogicAbstract : CardListLogicAbstract, IBench
    {
        protected BenchLogicAbstract()
            : base(new()) { }

        public int MaxBenchSpots { get; set; } = 5;
        int IBench.MaxBenchSpots => MaxBenchSpots;
        public abstract void ReplaceInPlace(
            IPokemonCardLogic oldPokemon,
            IPokemonCardLogic newPokemon
        );
        public bool Full
        {
            get => CardCount >= MaxBenchSpots;
        }
    }

    class Bench : BenchLogicAbstract
    {
        public Bench()
            : base() { }

        public override event Action<List<ICard>> CardCountChanged;

        public override void OnCardCountChanged()
        {
            CardCountChanged?.Invoke(((ICardList)this).Cards);
        }

        public override void AddCards(List<ICardLogic> cards)
        {
            foreach (var card in cards)
            {
                card.OwnerPositionKnowledge = PositionKnowledge.Known;
                card.OpponentPositionKnowledge = PositionKnowledge.Known;
            }
            base.AddCards(cards);
        }

        public override void ReplaceInPlace(
            IPokemonCardLogic oldPokemon,
            IPokemonCardLogic newPokemon
        )
        {
            var index = Cards.IndexOf(oldPokemon);
            Cards[index] = newPokemon;
            OnCardCountChanged();
        }
    }
}
