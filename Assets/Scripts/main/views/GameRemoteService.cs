using System;
using System.Collections.Generic;
using gamecore.card;
using gamecore.game;

namespace gameview
{
    public class GameRemoteService
    {
        public IGameController GameController { get; set; }
        private GameManager _gameManager;
        private List<ICard> _playableCards = new();

        public GameRemoteService(GameManager gameManager)
        {
            _gameManager = gameManager;
            InitializeGame();
        }

        private void InitializeGame()
        {
            GameController = new GameBuilder()
                .WithPlayer1Decklist(CreateDeckList())
                .WithPlayer2Decklist(CreateDeckList())
                .Build();
            GameController.NotifyPlayer1 += HandlePlayer1Interactions;
            GameController.NotifyPlayer2 += HandlePlayer2Interactions;
            GameController.NotifyGeneral += HandleGeneralInteractions;
        }

        public void StartGame()
        {
            GameController.SetUpGame();
        }

        private void HandlePlayer1Interactions(object sender, List<GameInteraction> interactions)
        {
            HandleInteraction(interactions);
        }

        private void HandlePlayer2Interactions(object sender, List<GameInteraction> interactions)
        {
            HandleInteraction(interactions);
        }

        private void HandleGeneralInteractions(object sender, List<GameInteraction> interactions)
        {
            HandleInteraction(interactions);
        }

        private void HandleInteraction(List<GameInteraction> interactions)
        {
            foreach (var interaction in interactions)
            {
                if (interaction.Type == GameInteractionType.PlayCard)
                {
                    HandlePlayCard(interaction);
                }
                else if (interaction.Type == GameInteractionType.EndTurn)
                    _gameManager.EnableEndTurn(interaction.GameControllerMethod, OnInteract);
                else if (interaction.Type == GameInteractionType.SelectActivePokemon)
                    HandleSelectActivePokemon(interaction);
                else if (interaction.Type == GameInteractionType.SetUpGame)
                    interaction.GameControllerMethod.Invoke();
                else if (interaction.Type == GameInteractionType.ConfirmMulligans)
                    HandleConfirmMulligans(interaction);
                else if (interaction.Type == GameInteractionType.SelectMulligans)
                    HandleSelectMulligans(interaction);
                else
                    throw new NotImplementedException();
            }
        }

        private void HandlePlayCard(GameInteraction interaction)
        {
            _playableCards.Add(interaction.Card);
            var cardView = CardViewRegistry.INSTANCE.Get(interaction.Card);
            cardView.SetPlayable(
                true,
                new DragBehaviour(() =>
                {
                    OnInteract();
                    interaction.GameControllerMethod.Invoke();
                })
            );
        }

        private void HandleSelectActivePokemon(GameInteraction interaction)
        {
            _playableCards.Add(interaction.Card);
            var cardView = CardViewRegistry.INSTANCE.Get(interaction.Card);
            cardView.SetPlayable(
                true,
                new ClickBehaviour(() =>
                {
                    OnInteract();
                    _gameManager
                        .PlayerActiveSpots[interaction.Card.Owner]
                        .SetActivePokemon(cardView);

                    interaction.GameControllerMethod.Invoke();
                })
            );
        }

        private void HandleConfirmMulligans(GameInteraction interaction)
        {
            _gameManager.EnablePlayerHandViews();
            _gameManager.ShowGameState();
            foreach (
                var player in new List<IPlayer>()
                {
                    GameController.Game.Player1,
                    GameController.Game.Player2,
                }
            )
            {
                _gameManager.ShowMulligan(
                    player,
                    GameController.Game.Mulligans[player],
                    () =>
                    {
                        OnInteract();
                        interaction.GameControllerMethod.Invoke();
                    }
                );
            }
        }

        private void HandleSelectMulligans(GameInteraction interaction)
        {
            _gameManager.ShowMulliganSelector(
                interaction.PossibleTargets,
                (selected) =>
                {
                    OnInteract();
                    interaction.GameControllerMethodWithTargets.Invoke(new() { selected });
                }
            );
        }

        private void OnInteract()
        {
            _gameManager.DisableEndTurn();
            ClearPlayableCards();
        }

        private void ClearPlayableCards()
        {
            foreach (var cardView in CardViewRegistry.INSTANCE.GetAll(_playableCards))
            {
                cardView.SetPlayable(false, null);
            }
            _playableCards.Clear();
        }

        private Dictionary<string, int> CreateDeckList()
        {
            return new Dictionary<string, int> { { "bill", 57 }, { "TWM128", 3 } };
        }
    }
}
