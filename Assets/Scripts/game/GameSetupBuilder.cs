using System.Collections.Generic;
using System.Linq;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game;
using gamecore.game.action;
using UnityEngine;

namespace gamecore
{
    public class GameSetupBuilder
    {
        public IPlayer Player1 { get; private set; }
        public IPlayer Player2 { get; private set; }

        private Dictionary<IPlayer, List<List<ICard>>> mulligans = new();

        public GameSetupBuilder WithPlayer1(IPlayer player)
        {
            Player1 = player;
            return this;
        }

        public GameSetupBuilder WithPlayer2(IPlayer player)
        {
            Player2 = player;
            return this;
        }

        public List<List<ICard>> GetMulligansForPlayer(IPlayer player)
        {
            return mulligans.TryGetValue(player, out var playerMulligans)
                ? playerMulligans
                : new List<List<ICard>>();
        }

        public void Setup()
        {
            mulligans.Add(Player1, new List<List<ICard>>());
            mulligans.Add(Player2, new List<List<ICard>>());
            var numberMulligansPlayer1 = DrawUntilBasicPokemon(Player1);
            var numberMulligansPlayer2 = DrawUntilBasicPokemon(Player2);
            Debug.Log($"Player 1 had {numberMulligansPlayer1} mulligans.");
            Debug.Log($"Player 2 had {numberMulligansPlayer2} mulligans.");
        }

        private int DrawUntilBasicPokemon(IPlayer player)
        {
            int count = 0;
            while (!DrawStartHand(player))
            {
                count++;
                mulligans[player].Add(player.Hand.Cards.GetRange(0, player.Hand.GetCardCount()));
                player.Deck.AddCards(player.Hand.Cards);
                player.Hand.Clear();
            }
            return count;
        }

        private bool DrawStartHand(IPlayer player)
        {
            player.Deck.Shuffle();
            ActionSystem.INSTANCE.Perform(new DrawCardGA(7, player));
            return HasBasicPokemon(player);
        }

        private bool HasBasicPokemon(IPlayer player)
        {
            return player.Hand.Cards.Any(card =>
                card is IPokemonCard pokemonCard && pokemonCard.Stage == Stage.Basic
            );
        }
    }
}
