using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game;
using gamecore.game.action;
using UnityEngine;

namespace gamecore
{
    class GameSetupBuilder
    {
        public IPlayerLogic Player1 { get; private set; }
        public IPlayerLogic Player2 { get; private set; }

        public Dictionary<IPlayer, List<List<ICardLogic>>> Mulligans { get; } = new();

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

        public async Task Setup()
        {
            Mulligans.Add(Player1, new List<List<ICardLogic>>());
            Mulligans.Add(Player2, new List<List<ICardLogic>>());
            var numberMulligansPlayer1 = DrawUntilBasicPokemon(Player1);
            var numberMulligansPlayer2 = DrawUntilBasicPokemon(Player2);
            Debug.Log($"Player 1 had {await numberMulligansPlayer1} mulligans.");
            Debug.Log($"Player 2 had {await numberMulligansPlayer2} mulligans.");
        }

        private async Task<int> DrawUntilBasicPokemon(IPlayerLogic player)
        {
            int count = 0;
            while (!await DrawStartHand(player))
            {
                count++;
                Mulligans[player].Add(player.Hand.Cards.GetRange(0, player.Hand.CardCount));
                player.Deck.AddCards(player.Hand.Cards);
                player.Hand.Clear();
            }
            return count;
        }

        private static async Task<bool> DrawStartHand(IPlayerLogic player)
        {
            player.Deck.Shuffle();
            await ActionSystem.INSTANCE.Perform(new DrawCardGA(7, player));
            return HasBasicPokemon(player);
        }

        private static bool HasBasicPokemon(IPlayerLogic player)
        {
            return player.Hand.Cards.Any(card =>
                card is IPokemonCard pokemonCard && pokemonCard.Stage == Stage.Basic
            );
        }
    }
}
