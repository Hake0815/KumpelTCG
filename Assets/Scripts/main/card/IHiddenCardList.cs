using System;
using System.Collections.Generic;

namespace gamecore.card
{
    public interface IHiddenCardList
    {
        int CardCount { get; }
        event Action CardCountChanged;
    }

    internal interface IHiddenCardListLogic : IHiddenCardList
    {
        List<ICardLogic> Draw(int amount);
        void Shuffle();
        void AddCards(List<ICardLogic> cards);
    }
}
