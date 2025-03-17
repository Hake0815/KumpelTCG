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
            player.CardDrawn += UpdateText;
        }

        public void UpdateText(object player, List<ICard> drawnCards)
        {
            Text.text = Deck.GetCardCount().ToString();
        }
    }
}
