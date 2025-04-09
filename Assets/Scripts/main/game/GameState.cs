using System;
using System.Collections.Generic;
using System.Linq;
using gamecore.card;
using UnityEngine;

namespace gamecore.game
{
    internal interface IGameState
    {
        IGameState AdvanceSuccesfully();
        List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        );
        void OnAdvanced(Game game);
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

        public void OnAdvanced(Game game) { }
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

        public void OnAdvanced(Game game)
        {
            if (_numberConfirmations == 0)
                game.AwaitGeneralInteraction();
        }
    }

    internal class SettingActivePokemonState : IGameState
    {
        private int _numberOfActivePokemonSelected = 0;

        public IGameState AdvanceSuccesfully()
        {
            _numberOfActivePokemonSelected++;
            if (_numberOfActivePokemonSelected == 2)
                return new SelectingMulliganCards();

            return this;
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            if (player.ActivePokemon != null)
                return new();
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

        public void OnAdvanced(Game game)
        {
            game.AwaitInteraction();
        }
    }

    internal class SelectingMulliganCards : IGameState
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
            var mulligans = gameController.Game.Mulligans;
            var numberOfMulligansOfPlayer = mulligans[player].Count;
            int mulliganDifference = 0;
            foreach (var mulligan in mulligans.Values)
            {
                mulliganDifference = Math.Max(
                    mulligan.Count - numberOfMulligansOfPlayer,
                    mulliganDifference
                );
            }

            if (mulliganDifference <= 0)
                return new();

            return new List<GameInteraction>()
            {
                new(
                    gameControllerMethodWithTargets: (targets) =>
                        gameController.SelectMulligans((int)targets[0], player),
                    type: GameInteractionType.SelectMulligans,
                    card: null,
                    possibleTargets: Enumerable
                        .Range(0, mulliganDifference + 1)
                        .Cast<object>()
                        .ToList(),
                    numberOfTargets: 1
                ),
            };
        }

        public void OnAdvanced(Game game)
        {
            var mulligans = game.Mulligans.Values.ToList();
            if (mulligans[0].Count != mulligans[1].Count)
                game.AwaitInteraction();
            else
                game.AdvanceGameState();
        }
    }

    internal class StartingGameState : IGameState
    {
        public IGameState AdvanceSuccesfully()
        {
            return new IdlePlayerTurnState();
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            return new();
        }

        public void OnAdvanced(Game game)
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
            AddPlayCardInteractions(interactions, gameController, playableCards);
            interactions.Add(
                new GameInteraction(gameController.EndTurn, GameInteractionType.EndTurn)
            );
            return interactions;
        }

        private List<ICardLogic> GetPlayableCardsFromHand(IPlayerLogic player)
        {
            var playableCards = new List<ICardLogic>();
            foreach (var card in player.Hand.Cards)
            {
                if (card.IsPlayable())
                    playableCards.Add(card);
            }

            return playableCards;
        }

        private void AddPlayCardInteractions(
            List<GameInteraction> interactions,
            GameController gameController,
            List<ICardLogic> playableCards
        )
        {
            foreach (var card in playableCards)
            {
                interactions.Add(
                    new GameInteraction(
                        () => gameController.PlayCard(card),
                        GameInteractionType.PlayCard,
                        card
                    )
                );
            }
        }

        public void OnAdvanced(Game game)
        {
            game.AwaitInteraction();
        }
    }
}
