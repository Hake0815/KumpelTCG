using System;
using System.Collections.Generic;
using System.Linq;
using gamecore.card;
using gamecore.game;
using UnityEngine;

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
                switch (interaction.Type)
                {
                    case GameInteractionType.PlayCard:
                        HandlePlayCard(interaction);
                        break;
                    case GameInteractionType.PlayCardWithTargets:
                        HandlePlayCardWithTargets(interaction);
                        break;
                    case GameInteractionType.PerformAttack:
                        HandlePerformAttack(interaction);
                        break;
                    case GameInteractionType.EndTurn:
                        _gameManager.EnableEndTurnButton(
                            interaction.GameControllerMethod,
                            OnInteract
                        );
                        break;
                    case GameInteractionType.SelectActivePokemon:
                        HandleSelectActivePokemon(interaction);
                        break;
                    case GameInteractionType.SetUpGame:
                        interaction.GameControllerMethod.Invoke();
                        break;
                    case GameInteractionType.SetupCompleted:
                        HandleSetupCompleted(interaction);
                        break;
                    case GameInteractionType.ConfirmMulligans:
                        HandleConfirmMulligans(interaction);
                        break;
                    case GameInteractionType.SelectMulligans:
                        HandleSelectMulligans(interaction);
                        break;
                    case GameInteractionType.Confirm:
                        _gameManager.EnableDoneButton(interaction.GameControllerMethod, OnInteract);
                        break;
                    case GameInteractionType.GameOver:
                        _gameManager.ShowGameOver(
                            (interaction.Data[typeof(WinnerData)] as WinnerData).Winner
                        );
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private void HandleSetupCompleted(GameInteraction interaction)
        {
            _gameManager.EnablePlayerHandViews();
            _gameManager.ShowGameState();
            interaction.GameControllerMethod.Invoke();
        }

        private void HandlePlayCard(GameInteraction interaction)
        {
            var card = (interaction.Data[typeof(InteractionCard)] as InteractionCard).Card;
            _playableCards.Add(card);
            var cardView = CardViewRegistry.INSTANCE.Get(card);
            cardView.SetPlayable(
                true,
                new DragBehaviour(() =>
                {
                    OnInteract();
                    interaction.GameControllerMethod.Invoke();
                })
            );
        }

        private void HandlePlayCardWithTargets(GameInteraction interaction)
        {
            var card = (interaction.Data[typeof(InteractionCard)] as InteractionCard).Card;
            _playableCards.Add(card);
            var cardView = CardViewRegistry.INSTANCE.Get(card);
            cardView.SetPlayable(
                true,
                new DragToTargetBehaviour(
                    (targets) =>
                    {
                        OnInteract();
                        interaction.GameControllerMethodWithTargets.Invoke(targets);
                    },
                    (interaction.Data[typeof(TargetData)] as TargetData)
                        .PossibleTargets.AsEnumerable()
                        .Select(card => CardViewRegistry.INSTANCE.Get(card as ICard))
                        .ToList()
                )
            );
        }

        private void HandlePerformAttack(GameInteraction interaction)
        {
            var card = (interaction.Data[typeof(InteractionCard)] as InteractionCard).Card;
            _playableCards.Add(card);
            var cardView = CardViewRegistry.INSTANCE.Get(card);
            cardView.AddAttack(
                (interaction.Data[typeof(AttackData)] as AttackData).Attack,
                () =>
                {
                    OnInteract();
                    interaction.GameControllerMethod.Invoke();
                }
            );
            var clickBehaviour = new ClickBehaviour(cardView.ShowAttacks);
            cardView.SetPlayable(true, clickBehaviour);
        }

        private void HandleSelectActivePokemon(GameInteraction interaction)
        {
            var card = (interaction.Data[typeof(InteractionCard)] as InteractionCard).Card;
            _playableCards.Add(card);
            var cardView = CardViewRegistry.INSTANCE.Get(card);
            var clickBehaviour = new ClickBehaviour(() =>
            {
                OnInteract();
                _gameManager.PlayerActiveSpots[card.Owner].SetActivePokemon(cardView);

                interaction.GameControllerMethod.Invoke();
            });
            cardView.SetPlayable(true, clickBehaviour);
        }

        private void HandleConfirmMulligans(GameInteraction interaction)
        {
            var mulligans = (interaction.Data[typeof(MulliganData)] as MulliganData).Mulligans;
            if (mulligans.Count == 0)
            {
                OnInteract();
                interaction.GameControllerMethod.Invoke();
                return;
            }
            foreach (var mulliganEntry in mulligans)
            {
                _gameManager.ShowMulligan(
                    mulliganEntry.Key,
                    mulliganEntry.Value,
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
                (interaction.Data[typeof(TargetData)] as TargetData).PossibleTargets,
                (selected) =>
                {
                    OnInteract();
                    interaction.GameControllerMethodWithTargets.Invoke(new() { selected });
                }
            );
        }

        private void OnInteract()
        {
            _gameManager.DisableButton();
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
            return new Dictionary<string, int>
            {
                { "bill", 16 },
                { "TWM128", 8 },
                { "FireNRG", 17 },
                { "PsychicNRG", 17 },
            };
        }
    }
}
