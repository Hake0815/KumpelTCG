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
            GameController.NotifyPlayer1 += HandlePlayer2Interactions;
        }

        public void StartGame()
        {
            GameController.SetUpGame();
        }

        private void HandlePlayer1Interactions(object sender, List<GameInteraction> interactions)
        {
            HandleInteraction(interactions, GameController.Game.Player1);
        }

        private void HandlePlayer2Interactions(object sender, List<GameInteraction> interactions)
        {
            HandleInteraction(interactions, GameController.Game.Player2);
        }

        private void HandleInteraction(List<GameInteraction> interactions, IPlayer player)
        {
            foreach (var interaction in interactions)
            {
                if (interaction.Type == GameInteractionType.PlayCard)
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
                else if (interaction.Type == GameInteractionType.EndTurn)
                    _gameManager.EnableEndTurn(interaction.GameControllerMethod, OnInteract);
                else if (interaction.Type == GameInteractionType.SelectActivePokemon)
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
                else if (interaction.Type == GameInteractionType.SetUpGame)
                    interaction.GameControllerMethod.Invoke();
                else if (interaction.Type == GameInteractionType.ConfirmMulligans)
                {
                    _gameManager.ShowGameState();
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
                else
                    throw new NotImplementedException();
            }
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
            return new Dictionary<string, int> { { "bill", 56 }, { "TWM128", 4 } };
        }
    }
}
