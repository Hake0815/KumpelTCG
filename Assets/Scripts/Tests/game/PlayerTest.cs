using System.Collections.Generic;
using gamecore.card;
using Moq;
using NUnit.Framework;

namespace gamecore.game
{
    public class PlayerTest
    {
        private readonly Mock<DeckLogicAbstract> deck = new();

        private readonly ICardLogic card = Mock.Of<ICardLogic>();

        private Player Player;

        [SetUp]
        public void SetUp()
        {
            Player = new Player() { Deck = deck.Object };
            deck.Setup(d => d.Draw(1)).Returns(new List<ICardLogic> { card });
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

            Assert.That(Player.Hand.CardCount, Is.EqualTo(1));
            Assert.That(Player.Hand.Cards, Contains.Item(card));
        }
    }
}
