using System;
using gamecore.game;

namespace gamecore.card
{
    public interface ICard
    {
        event Action CardDiscarded;
        IPlayer Owner { get; }

        string Name { get; }
        string Id { get; }
        bool IsTrainerCard();
        bool IsPokemonCard();
    }

    internal interface ICardLogic : ICard
    {
        ICardData CardData { get; }
        string ICard.Name
        {
            get => CardData.Name;
        }
        string ICard.Id
        {
            get => CardData.Id;
        }
        new IPlayerLogic Owner { get; }
        IPlayer ICard.Owner => Owner;
        void Play();
        bool IsPlayable();
        void Discard();
    }
}
