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
        int CompareTo(ICard other, Predicate<ICard> _cardCondition)
        {
            if (_cardCondition is not null && _cardCondition(this) && !_cardCondition(other))
                return 1;
            if (_cardCondition is not null && !_cardCondition(this) && _cardCondition(other))
                return -1;
            if (GetType() == other.GetType())
                return Name.CompareTo(other.Name);
            if (IsPokemonCard() && !other.IsPokemonCard())
                return 1;
            if (!IsPokemonCard() && other.IsPokemonCard())
                return -1;
            if (IsTrainerCard() && !other.IsTrainerCard())
                return 1;
            if (!IsTrainerCard() && other.IsTrainerCard())
                return -1;
            return 0;
        }
        int CompareTo(ICard other)
        {
            return CompareTo(other, null);
        }
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
