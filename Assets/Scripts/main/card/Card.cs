using System;
using System.Collections.Generic;
using gamecore.game;
using Newtonsoft.Json;

namespace gamecore.card
{
    public interface ICard
    {
        int DeckId { get; }
        event Action CardDiscarded;
        IPlayer Owner { get; }

        string Name { get; }
        string Id { get; }
        bool IsTrainerCard();
        bool IsPokemonCard();
        bool IsEnergyCard();
    }

    internal interface ICardLogic : ICard
    {
        new IPlayerLogic Owner { get; }
        IPlayer ICard.Owner => Owner;
        void Play();
        bool IsPlayable();
        void PlayWithTargets(List<ICardLogic> targets);
        bool IsPlayableWithTargets();
        List<ICardLogic> GetPossibleTargets();
        int GetNumberOfTargets();
        void Discard();
    }
}
