using System;
using System.Collections.Generic;
using gamecore.card;

namespace gamecore.game
{
    public class GameBuilder
    {
        private readonly Player _player1 = new() { Name = "Player1" };
        private readonly Player _player2 = new() { Name = "Player2" };

        public GameBuilder WithPlayer1(string playerName1)
        {
            _player1.Name = playerName1;
            return this;
        }

        public GameBuilder WithPlayer2(string playerName2)
        {
            _player2.Name = playerName2;
            return this;
        }

        public GameBuilder WithPlayer1Decklist(Dictionary<string, int> decklist)
        {
            _player1.Deck = CreateDeckFromDecklist(decklist, _player1);
            return this;
        }

        public GameBuilder WithPlayer2Decklist(Dictionary<string, int> decklist)
        {
            _player2.Deck = CreateDeckFromDecklist(decklist, _player2);
            return this;
        }

        private static Deck CreateDeckFromDecklist(Dictionary<string, int> decklist, Player player)
        {
            var cards = new List<ICardLogic>();
            var deckId = 0;
            foreach (var entry in decklist)
            {
                cards.AddRange(CardFactory.CreateCard(entry.Key, player, entry.Value, deckId));
                deckId += entry.Value;
            }
            return new Deck(cards);
        }

        public IGameController Build()
        {
            _player1.Opponent = _player2;
            _player2.Opponent = _player1;
            var game = new Game(_player1, _player2);
            return new GameController(game);
        }
    }
}
