using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class CardViewRegistry
{
    private static readonly Lazy<CardViewRegistry> lazy = new(() => new CardViewRegistry());
    public static CardViewRegistry INSTANCE => lazy.Value;

    private CardViewRegistry() { }

    private readonly Dictionary<ICard, CardView> regsitry = new();

    public void Register(CardView cardView)
    {
        regsitry[cardView.Card] = cardView;
    }

    public void Unregister(ICard card)
    {
        regsitry.Remove(card);
    }

    public CardView Get(ICard card)
    {
        return regsitry[card];
    }

    public List<CardView> GetAll(ICollection<ICard> cards)
    {
        List<CardView> cardViews = new();
        foreach (var card in cards)
        {
            cardViews.Add(regsitry[card]);
        }
        return cardViews;
    }
}
