using System.Collections.Generic;
using Moq;
using NUnit.Framework;

public class PlayerTest
{
    private readonly Mock<IDeck> deck = new();

    private readonly Card card = CardTestFactory.Create();

    private Player Player;

    [SetUp]
    public void SetUp()
    {
        Player = new Player(deck.Object);
        deck.Setup(d => d.Draw()).Returns(card);
    }

    [Test]
    public void ShouldDrawFromDeck()
    {
        Player.Draw();

        deck.Verify(d => d.Draw());
    }

    [Test]
    public void ShouldDrawCardIntoHand()
    {
        Player.Draw();

        Assert.Contains(card, Player.Hand);
    }
}
