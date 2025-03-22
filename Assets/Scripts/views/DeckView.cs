using System.Collections.Generic;
using gamecore.card;
using gamecore.game;
using TMPro;
using UnityEngine;

namespace gameview
{
    public class DeckView : MonoBehaviour
    {
        public IDeck Deck { get; private set; }
        public TMP_Text Text { get; private set; }

        public void SetUp(IPlayer player)
        {
            Deck = player.Deck;
            Text = GetComponentInChildren<TMP_Text>();
            Text.text = Deck.GetCardCount().ToString();
            Deck.CardCountChanged += UpdateText;
        }

        public void UpdateText()
        {
            Text.text = Deck.GetCardCount().ToString();
        }
    }
}
