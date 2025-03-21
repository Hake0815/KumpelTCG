using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using gamecore.game;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace gamecore.card
{
    public class DeckTest
    {
        private readonly ICard firstCard = Mock.Of<ICard>();
        private readonly ICard secondCard = Mock.Of<ICard>();

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
}
