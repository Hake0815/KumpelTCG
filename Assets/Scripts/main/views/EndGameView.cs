using System;
using gamecore.game;
using TMPro;
using UnityEngine;

namespace gameview
{
    public class EndGameView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text;

        public void ShowWinner(IPlayer winner)
        {
            if (winner == null)
            {
                _text.text = "Draw.";
            }
            else
            {
                _text.text = winner.Name + " wins!";
            }
        }
    }
}
