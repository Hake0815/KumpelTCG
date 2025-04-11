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
        public event Action CardCountChanged;

        public Bench()
        {
            ActionSystem.INSTANCE.AttachPerformer(this);
        }

        public void OnCardCountChanged()
        {
            CardCountChanged?.Invoke();
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
