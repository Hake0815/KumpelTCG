using System;
using System.Collections.Generic;
using gamecore.game;
using Newtonsoft.Json;

namespace gamecore.card
{
    public interface ICard
    {
        int DeckId { get; }
        IPlayer Owner { get; }

        string Name { get; }
        string Id { get; }
        bool IsTrainerCard();
        bool IsSupporterCard();
        bool IsItemCard();
        bool IsPokemonCard();
        bool IsEnergyCard();
        bool IsBasicEnergyCard();
        int CompareTo(ICard other, Predicate<ICard> _cardCondition)
        {
            if (_cardCondition is not null && _cardCondition(this) && !_cardCondition(other))
                return 1;
            if (_cardCondition is not null && !_cardCondition(this) && _cardCondition(other))
                return -1;
            return CompareToCardByType(other);
        }

        int CompareToCardByType(ICard other)
        {
            if (GetType() == other.GetType())
                return CompareToEqualType(other);
            if (IsPokemonCard() && !other.IsPokemonCard())
                return 1;
            if (!IsPokemonCard() && other.IsPokemonCard())
                return -1;
            if (IsSupporterCard() && !other.IsSupporterCard())
                return 1;
            if (!IsSupporterCard() && other.IsSupporterCard())
                return -1;
            if (IsItemCard() && !other.IsItemCard())
                return 1;
            if (!IsItemCard() && other.IsItemCard())
                return -1;
            return 0;
        }

        int CompareToEqualType(ICard other)
        {
            var result = Name.CompareTo(other.Name);
            if (result == 0)
                return DeckId.CompareTo(other.DeckId);
            return result;
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
