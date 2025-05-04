using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    internal class Bench : IBenchLogic
    {
        public List<ICardLogic> Cards { get; } = new();
        public int MaxBenchSpots { get; set; } = 5;

        public event Action CardCountChanged;

        public void OnCardCountChanged()
        {
            CardCountChanged?.Invoke();
        }
    }
}
