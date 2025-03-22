using System;
using gamecore.game;

namespace gamecore.card
{
    public interface ICard
    {
        public ICardData CardData { get; }
        public IPlayer Owner { get; }
        public event Action CardDiscarded;

        public string Name
        {
            get => CardData.Name;
        }
        public string Id
        {
            get => CardData.Id;
        }
        public void Play();
        public bool IsPlayable();
        public void Discard();
        public bool IsTrainerCard();
    }
}
