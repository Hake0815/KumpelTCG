using System.Collections.Generic;
using gamecore.card;
using Moq;
using NUnit.Framework;

namespace gamecore.game
{
    public class PlayerTest
    {
        private readonly Mock<IDeck> deck = new();

        private readonly ICard card = Mock.Of<ICard>();

        private Player Player;

        [SetUp]
        public void SetUp()
        {
            Player = new Player(deck.Object);
            deck.Setup(d => d.Draw(1)).Returns(new List<ICard> { card });
        }

        [Test]
        public void ShouldDrawFromDeck()
        {
            Player.Draw(1);

            deck.Verify(d => d.Draw(1));
        }

        [Test]
        public void ShouldDrawCardIntoHand()
        {
            Player.Draw(1);

            Assert.That(Player.Hand.GetCardCount(), Is.EqualTo(1));
            Assert.That(Player.Hand.Cards, Contains.Item(card));
        }
    }
}
