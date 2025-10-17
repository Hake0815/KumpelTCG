using System.Collections.Generic;
using gamecore.game;
using Moq;
using NUnit.Framework;

namespace gamecore.card
{
    public class DeckTest
    {
        private readonly ICardLogic firstCard = Mock.Of<ICardLogic>();
        private readonly ICardLogic secondCard = Mock.Of<ICardLogic>();

        [Test]
        public void CardCountShouldBeTwo()
        {
            SetUpDeck(out Deck deck);

            var cardCount = deck.CardCount;

            Assert.That(cardCount, Is.EqualTo(2));
        }

        [Test]
        public void DrawShouldYieldFirstCard()
        {
            SetUpDeck(out Deck deck);

            var drawnCards = deck.Draw(1);

            Assert.That(drawnCards.Count, Is.EqualTo(1));
            Assert.That(drawnCards, Contains.Item(firstCard));
        }

        [Test]
        public void DrawShouldReduceCardCount()
        {
            SetUpDeck(out Deck deck);
            deck.Draw(1);

            var cardCount = deck.CardCount;

            Assert.That(cardCount, Is.EqualTo(1));
        }

        [Test]
        public void DrawAllRemainingCards()
        {
            SetUpDeck(out Deck deck);

            var drawnCards = deck.Draw(3);

            Assert.That(drawnCards.Count, Is.EqualTo(2));
            Assert.That(deck.CardCount, Is.EqualTo(0));
            Assert.That(drawnCards.Count, Is.EqualTo(2));
            Assert.That(drawnCards, Contains.Item(firstCard));
            Assert.That(drawnCards, Contains.Item(secondCard));
        }

        [Test]
        public void DrawShouldReturnEmptyListIfNoCardsAreLeft()
        {
            var deck = new Deck(new());

            var drawnCards = deck.Draw(3);

            Assert.That(drawnCards, Is.Empty);
        }

        private void SetUpDeck(out Deck deck)
        {
            deck = new Deck(new List<ICardLogic> { firstCard, secondCard });
        }
    }
}
