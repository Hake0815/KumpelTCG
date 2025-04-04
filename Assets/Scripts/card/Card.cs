using System;
using gamecore.game;

namespace gamecore.card
{
    public interface ICard
    {
        public ICardData CardData { get; }
        public event Action CardDiscarded;

        public string Name
        {
            get => CardData.Name;
        }
        public string Id
        {
            get => CardData.Id;
        }
    }

    internal interface ICardLogic : ICard
    {
        internal IPlayerLogic Owner { get; }
        internal void Play();
        internal bool IsPlayable();
        internal void Discard();
        internal bool IsTrainerCard();
        internal bool IsPokemonCard();
    }
}
