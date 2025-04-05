using System;
using gamecore.game;

namespace gamecore.card
{
    public interface ICard
    {
        ICardData CardData { get; }
        event Action CardDiscarded;

        string Name
        {
            get => CardData.Name;
        }
        string Id
        {
            get => CardData.Id;
        }
    }

    internal interface ICardLogic : ICard
    {
        IPlayerLogic Owner { get; }
        void Play();
        bool IsPlayable();
        void Discard();
        bool IsTrainerCard();
        bool IsPokemonCard();
    }
}
