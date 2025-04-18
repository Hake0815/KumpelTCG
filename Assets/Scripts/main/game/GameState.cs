using System;
using System.Collections.Generic;
using System.Linq;
using gamecore.card;

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
            return new SetupCompletedState();
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

    internal class SetupCompletedState : IGameState
    {
        public IGameState AdvanceSuccesfully()
        {
            return new ShowFirstMulliganState();
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            return new() { new(gameController.Confirm, GameInteractionType.SetupCompleted) };
        }

        public void OnAdvanced(Game game)
        {
            game.AwaitGeneralInteraction();
        }
    }

    internal class ShowFirstMulliganState : IGameState
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
            var mulligans = gameController.Game.Mulligans;
            var mulliganNumberToShow = mulligans.Values.ToList().Min(m => m.Count);

            var mulligansToShow = new Dictionary<IPlayer, List<List<ICard>>>();
            foreach (var p in mulligans.Keys)
                mulligansToShow.Add(p, mulligans[p].GetRange(0, mulliganNumberToShow));
            return new List<GameInteraction>()
            {
                new(
                    gameController.Confirm,
                    GameInteractionType.ConfirmMulligans,
                    new() { new MulliganData(mulligansToShow) }
                ),
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
                return new ShowSecondMulliganState();

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
            foreach (var basicPokemon in player.Hand.GetBasicPokemon())
            {
                interactions.Add(CreateGameInteraction(basicPokemon, gameController));
            }
            return interactions;
        }

        private GameInteraction CreateGameInteraction(
            ICardLogic basicPokemon,
            GameController gameController
        )
        {
            return new GameInteraction(
                () => gameController.SelectActivePokemon(basicPokemon),
                GameInteractionType.SelectActivePokemon,
                new() { new InteractionCard(basicPokemon) }
            );
        }

        public void OnAdvanced(Game game)
        {
            game.AwaitInteraction();
        }
    }

    internal class ShowSecondMulliganState : IGameState
    {
        public IGameState AdvanceSuccesfully()
        {
            return new SelectingMulliganCards();
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            var mulligans = gameController.Game.Mulligans;
            var mulliganNumberToSkip = mulligans.Values.ToList().Min(m => m.Count);

            var mulligansToShow = new Dictionary<IPlayer, List<List<ICard>>>();
            foreach (var p in mulligans.Keys)
                if (mulligans[p].Count > mulliganNumberToSkip)
                    mulligansToShow.Add(
                        p,
                        mulligans[p]
                            .GetRange(
                                mulliganNumberToSkip,
                                mulligans[p].Count - mulliganNumberToSkip
                            )
                    );
            return new List<GameInteraction>()
            {
                new(
                    gameController.Confirm,
                    GameInteractionType.ConfirmMulligans,
                    new() { new MulliganData(mulligansToShow) }
                ),
            };
        }

        public void OnAdvanced(Game game)
        {
            game.AwaitGeneralInteraction();
        }
    }

    internal class SelectingMulliganCards : IGameState
    {
        public IGameState AdvanceSuccesfully()
        {
            return new SelectBenchPokemonState();
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

            var targetData = new TargetData(
                numberOfTargets: 1,
                possibleTargets: Enumerable.Range(0, mulliganDifference + 1).Cast<object>().ToList()
            );
            return new List<GameInteraction>()
            {
                new(
                    gameControllerMethodWithTargets: (targets) =>
                        gameController.SelectMulligans((int)targets[0], player),
                    type: GameInteractionType.SelectMulligans,
                    data: new() { targetData }
                ),
            };
        }

        public void OnAdvanced(Game game)
        {
            game.SetPrizeCards();
            var mulligans = game.Mulligans.Values.ToList();
            if (mulligans[0].Count != mulligans[1].Count)
                game.AwaitInteraction();
            else
                game.AdvanceGameState();
        }
    }

    internal class SelectBenchPokemonState : IGameState
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
            foreach (var basicPokemon in player.Hand.GetBasicPokemon())
            {
                interactions.Add(CreateGameInteraction(basicPokemon, gameController));
            }
            interactions.Add(new(gameController.Confirm, GameInteractionType.Confirm));
            return interactions;
        }

        private GameInteraction CreateGameInteraction(
            ICardLogic basicPokemon,
            GameController gameController
        )
        {
            return new GameInteraction(
                () => gameController.PlayCard(basicPokemon),
                GameInteractionType.PlayCard,
                new() { new InteractionCard(basicPokemon) }
            );
        }

        public void OnAdvanced(Game game)
        {
            game.AwaitInteraction();
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

            var interactions = new List<GameInteraction>();
            AddPlayCardInteractions(interactions, gameController, player);
            AddPlayCardWithTargetsInteractions(interactions, gameController, player);
            interactions.Add(
                new GameInteraction(gameController.EndTurn, GameInteractionType.EndTurn)
            );
            return interactions;
        }

        private void AddPlayCardInteractions(
            List<GameInteraction> interactions,
            GameController gameController,
            IPlayerLogic player
        )
        {
            var playableCards = GetPlayableCardsFromHand(player);
            foreach (var card in playableCards)
            {
                interactions.Add(
                    new GameInteraction(
                        () => gameController.PlayCard(card),
                        GameInteractionType.PlayCard,
                        new() { new InteractionCard(card) }
                    )
                );
            }
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

        private void AddPlayCardWithTargetsInteractions(
            List<GameInteraction> interactions,
            GameController gameController,
            IPlayerLogic player
        )
        {
            var playableCards = GetPlayableCardsWithTargetFromHand(player);
            foreach (var card in playableCards)
            {
                var targets = card.GetTargets();
                interactions.Add(
                    new GameInteraction(
                        (selectedTargets) =>
                            gameController.PlayCardWithTargets(
                                card,
                                selectedTargets.AsEnumerable().Cast<ICardLogic>().ToList()
                            ),
                        GameInteractionType.PlayCardWithTargets,
                        new()
                        {
                            new InteractionCard(card),
                            new TargetData(
                                card.GetTargets().Count,
                                targets.AsEnumerable().Cast<object>().ToList()
                            ),
                        }
                    )
                );
            }
        }

        private List<ICardLogic> GetPlayableCardsWithTargetFromHand(IPlayerLogic player)
        {
            var playableCards = new List<ICardLogic>();
            foreach (var card in player.Hand.Cards)
            {
                if (card.IsPlayableWithTargets())
                    playableCards.Add(card);
            }
            return playableCards;
        }

        public void OnAdvanced(Game game)
        {
            game.AwaitInteraction();
        }
    }
}
