using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using gamecore.card;
using gamecore.game;

namespace gamecore
{
    class GameSetupBuilder
    {
        public IPlayerLogic Player1 { get; private set; }
        public IPlayerLogic Player2 { get; private set; }

        public Dictionary<IPlayerLogic, List<List<ICardLogic>>> Mulligans { get; } = new();

        public GameSetupBuilder WithPlayer1(IPlayerLogic player)
        {
            Player1 = player;
            return this;
        }

        public GameSetupBuilder WithPlayer2(IPlayerLogic player)
        {
            Player2 = player;
            return this;
        }

        public List<List<ICardLogic>> GetMulligansForPlayer(IPlayerLogic player)
        {
            return Mulligans.TryGetValue(player, out var playerMulligans)
                ? playerMulligans
                : new List<List<ICardLogic>>();
        }

        public void Setup()
        {
            Mulligans.Add(Player1, new List<List<ICardLogic>>());
            Mulligans.Add(Player2, new List<List<ICardLogic>>());
            DrawUntilBasicPokemon(Player1);
            DrawUntilBasicPokemon(Player2);
        }

        private void DrawUntilBasicPokemon(IPlayerLogic player)
        {
            while (!DrawStartHand(player))
            {
                Mulligans[player].Add(player.Hand.Cards.GetRange(0, player.Hand.CardCount));
                player.Deck.AddCards(player.Hand.Cards);
                player.Hand.Clear();
            }
        }

        private static bool DrawStartHand(IPlayerLogic player)
        {
            player.Deck.Shuffle();
            var drawnCards = player.Draw(7);
            return HasBasicPokemon(drawnCards);
        }

        private static bool HasBasicPokemon(List<ICardLogic> startingHand)
        {
            return startingHand.Any(card =>
                card is IPokemonCard pokemonCard && pokemonCard.Stage == Stage.Basic
            );
        }
    }
}
