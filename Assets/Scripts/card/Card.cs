using System;
using gamecore.game;

namespace gamecore.card
{
    public interface ICard
    {
        ICardData CardData { get; }
        event Action CardDiscarded;
        IPlayer Owner { get; }

        string Name
        {
            get => CardData.Name;
        }
        string Id
        {
            get => CardData.Id;
        }
        bool IsTrainerCard();
        bool IsPokemonCard();
    }

    internal interface ICardLogic : ICard
    {
        new IPlayerLogic Owner { get; }
        void Play();
        bool IsPlayable();
        void Discard();
    }
}
