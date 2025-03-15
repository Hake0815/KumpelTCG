using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DeckTest
{
    private readonly ICard firstCard = new CardDummy(CardDatabase.cardDataList[0]);
    private readonly ICard secondCard = new CardDummy(CardDatabase.cardDataList[1]);

    [Test]
    public void CardCountSHouldBeTwo()
    {
        SetUpDeck(out Deck deck);

        var cardCount = deck.GetCardCount();

        Assert.AreEqual(cardCount, 2);
    }

    [Test]
    public void DrawShouldYieldCard()
    {
        SetUpDeck(out Deck deck);

        var drawnCard = deck.Draw();

        Assert.AreEqual(drawnCard, firstCard);
    }

    [Test]
    public void DrawShouldReduceCardCount()
    {
        SetUpDeck(out Deck deck);
        deck.Draw();

        var cardCount = deck.GetCardCount();

        Assert.AreEqual(cardCount, 1);
    }

    private void SetUpDeck(out Deck deck)
    {
        deck = new Deck();
        deck.SetUp(new List<ICard> { firstCard, secondCard });
    }
}
