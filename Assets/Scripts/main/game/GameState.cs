using System;
using System.Collections;
using System.Collections.Generic;
using gamecore.card;
using NUnit.Framework.Constraints;

namespace gamecore.game
{
    internal interface IGameState
    {
        IGameState AdvanceSuccesfully();
        List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        );
        void OnEnter(Game game);
    }

    internal class CreatedState : IGameState
    {
        public IGameState AdvanceSuccesfully()
        {
            return new ShowMulliganState();
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            var gameInteraction = new GameInteraction(
                gameController.SetUpGame,
                GameInteractionType.SetUpGame
            );
            return new List<GameInteraction>() { gameInteraction };
        }

        public void OnEnter(Game game) { }
    }

    internal class ShowMulliganState : IGameState
    {
        private int _numberConfirmations = 0;

        public IGameState AdvanceSuccesfully()
        {
            _numberConfirmations++;
            if (_numberConfirmations == 2)
                return new SettingActivePokemonState();

            return this;
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            return new List<GameInteraction>()
            {
                new(gameController.Confirm, GameInteractionType.ConfirmMulligans),
            };
        }

        public void OnEnter(Game game)
        {
            if (_numberConfirmations == 0)
                game.AwaitInteraction();
        }
    }

    internal class SettingActivePokemonState : IGameState
    {
        public IGameState AdvanceSuccesfully()
        {
            return new StartingGameState();
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            var interactions = new List<GameInteraction>();
            foreach (var basicPokemon in GetBasicPokemon(player))
            {
                interactions.Add(CreateGameInteraction(basicPokemon, gameController));
            }
            return interactions;
        }

        private List<ICardLogic> GetBasicPokemon(IPlayerLogic player)
        {
            var basicPokemon = new List<ICardLogic>();
            foreach (var card in player.Hand.Cards)
            {
                if (card is PokemonCard pokemonCard && pokemonCard.Stage == Stage.Basic)
                {
                    basicPokemon.Add(card);
                }
            }
            return basicPokemon;
        }

        private GameInteraction CreateGameInteraction(
            ICardLogic basicPokemon,
            GameController gameController
        )
        {
            return new GameInteraction(
                () => gameController.SelectActivePokemon(basicPokemon),
                GameInteractionType.SelectActivePokemon,
                basicPokemon
            );
        }

        public void OnEnter(Game game) { }
    }

    internal class StartingGameState : IGameState
    {
        public IGameState AdvanceSuccesfully()
        {
            throw new NotImplementedException();
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            return new();
        }

        public void OnEnter(Game game)
        {
            game.StartGame();
            game.AdvanceGameState();
        }
    }

    internal class IdlePlayerTurnState : IGameState
    {
        public IGameState AdvanceSuccesfully()
        {
            return this;
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            if (!player.IsActive)
                return new();

            var playableCards = GetPlayableCardsFromHand(player);
            var interactions = new List<GameInteraction>();
            AddPlayCardInteractions(gameController, playableCards);
            interactions.Add(
                new GameInteraction(gameController.EndTurn, GameInteractionType.EndTurn)
            );
            return interactions;
        }

        private static List<ICardLogic> GetPlayableCardsFromHand(IPlayerLogic player)
        {
            var playableCards = new List<ICardLogic>();
            foreach (var card in player.Hand.Cards)
            {
                if (card.IsPlayable())
                    playableCards.Add(card);
            }

            return playableCards;
        }

        private static void AddPlayCardInteractions(
            GameController gameController,
            List<ICardLogic> playableCards
        )
        {
            foreach (var card in playableCards)
            {
                new GameInteraction(
                    () => gameController.PlayCard(card),
                    GameInteractionType.PlayCard,
                    card
                );
            }
        }

        public void OnEnter(Game game)
        {
            game.AwaitInteraction();
        }
    }
}
