using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace gamecore.card
{
    public class DiscardPileTest
    {
        private readonly ICard firstCard = Mock.Of<ICard>();
        private readonly ICard secondCard = Mock.Of<ICard>();

        [Test]
        public void ShouldReturnLastCard()
        {
            var discardPile = new DiscardPile();
            discardPile.AddCards(new List<ICard> { firstCard, secondCard });

            var lastCard = discardPile.GetLastCard();

            Assert.That(lastCard, Is.EqualTo(secondCard));
        }

        [Test]
        public void ShouldReturnLastCard2()
        {
            var discardPile = new DiscardPile();
            discardPile.AddCards(new List<ICard> { firstCard });
            discardPile.AddCards(new List<ICard> { secondCard });

            var lastCard = discardPile.GetLastCard();

            Assert.That(lastCard, Is.EqualTo(secondCard));
        }

        [Test]
        public void ShouldReturnNullIfNoCards()
        {
            var discardPile = new DiscardPile();

            var lastCard = discardPile.GetLastCard();

            Assert.That(lastCard, Is.Null);
        }
    }
}
