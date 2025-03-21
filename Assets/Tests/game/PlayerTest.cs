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
}
