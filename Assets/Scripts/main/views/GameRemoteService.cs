using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using gamecore.card;
using gamecore.game;
using UnityEngine.UI;

namespace gameview
{
    public class GameRemoteService
    {
        private IGameController _gameController;
        private readonly GameManager _gameManager;
        private readonly List<ICard> _playableCards = new();
        private readonly List<ICard> _selectedCards = new();

        public GameRemoteService(GameManager gameManager)
        {
            _gameManager = gameManager;
            InitializeGame();
        }

        private void InitializeGame()
        {
            var gameLogFile = "action_log.json";
            _gameController = IGameController.Create(gameLogFile);
            _gameController.NotifyPlayer1 += HandlePlayer1Interactions;
            _gameController.NotifyPlayer2 += HandlePlayer2Interactions;
            _gameController.NotifyGeneral += HandleGeneralInteractions;
            if (File.Exists(gameLogFile) && File.ReadAllText(gameLogFile).Length > 0)
            {
                _gameController.RecreateGameFromLog();
            }
            else
            {
                _gameController.CreateGame(
                    CreateDeckList(),
                    CreateDeckList(),
                    "Player 1",
                    "Player 2"
                );
            }
            _gameManager.SetUpPlayerViews(
                _gameController.Game.Player1,
                _gameController.Game.Player2
            );
            _gameManager.EnablePlayerHandViews();
            var cachedSpeed = AnimationSpeedHolder.AnimationSpeed;
            AnimationSpeedHolder.AnimationSpeed = 0.0f;
            _gameManager.ShowGameState();
            UIQueue.INSTANCE.Queue(
                (callback) =>
                {
                    AnimationSpeedHolder.AnimationSpeed = cachedSpeed;
                    _gameController.StartGame();
                    callback.Invoke();
                }
            );
        }

        private static Dictionary<string, int> CreateDeckList()
        {
            return new Dictionary<string, int>
            {
                { "professorsResearch", 8 },
                { "TWM128", 8 },
                { "TWM129", 8 },
                { "ultraBall", 12 },
                { "nightStretcher", 10 },
                { "FireNRG", 7 },
                { "PsychicNRG", 7 },
            };
        }

        private void HandlePlayer1Interactions(object sender, List<GameInteraction> interactions)
        {
            UIQueue.INSTANCE.Queue(action =>
            {
                HandleInteraction(interactions);
                action.Invoke();
            });
        }

        private void HandlePlayer2Interactions(object sender, List<GameInteraction> interactions)
        {
            UIQueue.INSTANCE.Queue(action =>
            {
                HandleInteraction(interactions);
                action.Invoke();
            });
        }

        private void HandleGeneralInteractions(object sender, List<GameInteraction> interactions)
        {
            UIQueue.INSTANCE.Queue(action =>
            {
                HandleInteraction(interactions);
                action.Invoke();
            });
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
                    case GameInteractionType.PerformAbility:
                        HandlePerformAbility(interaction);
                        break;
                    case GameInteractionType.Retreat:
                        HandleRetreat(interaction);
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
                    case GameInteractionType.SetupCompleted:
                        SimpleProceed(interaction);
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
                    case GameInteractionType.SelectCards:
                        HandleSelectCards(interaction);
                        break;
                    case GameInteractionType.SetPrizeCards:
                        SimpleProceed(interaction);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private void HandleSelectCards(GameInteraction interaction)
        {
            var targetData =
                interaction.Data[typeof(ConditionalTargetData)] as ConditionalTargetData;
            var selectFromData = interaction.Data[typeof(SelectFromData)] as SelectFromData;
            switch (selectFromData.SelectFrom)
            {
                case SelectFrom.InPlay:
                    // Nothing to do here
                    break;
                case SelectFrom.Floating:
                    PrepareFloatingSelection(targetData.PossibleTargets);
                    break;
                case SelectFrom.Deck:
                    PrepareSearch(selectFromData.SelectionSource, targetData.PossibleTargets);
                    break;
                case SelectFrom.DiscardPile:
                    PrepareSearch(selectFromData.SelectionSource, targetData.PossibleTargets);
                    break;
                default:
                    throw new NotImplementedException();
            }

            SetUpSelection(
                targetData.ConditionOnSelection,
                targetData.PossibleTargets,
                interaction.GameControllerMethodWithTargets,
                targetData.IsQuickSelection
            );
        }

        private void PrepareFloatingSelection(List<ICard> possibleTargets)
        {
            _gameManager.ActivateFloatingSelectionView(possibleTargets);
        }

        private void PrepareSearch(List<ICard> cards, List<ICard> possibleTargets)
        {
            _gameManager.ActivateSearchView(cards, possibleTargets);
        }

        private static void SimpleProceed(GameInteraction interaction)
        {
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
                        .Select(card => CardViewRegistry.INSTANCE.Get(card))
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
            var clickBehaviour = new ClickBehaviour(cardView.ShowActivePokemonActions);
            cardView.SetPlayable(true, clickBehaviour);
        }

        private void HandlePerformAbility(GameInteraction interaction)
        {
            var card = (interaction.Data[typeof(InteractionCard)] as InteractionCard).Card;
            _playableCards.Add(card);
            var cardView = CardViewRegistry.INSTANCE.Get(card);
            cardView.AddAbility(
                (card as IPokemonCard).Ability,
                () =>
                {
                    OnInteract();
                    interaction.GameControllerMethod.Invoke();
                },
                card.Owner.ActivePokemon == card
            );
            if (card.Owner.ActivePokemon == card)
            {
                var clickBehaviour = new ClickBehaviour(cardView.ShowActivePokemonActions);
                cardView.SetPlayable(true, clickBehaviour);
            }
            else
            {
                var clickBehaviour = new ClickBehaviour(cardView.ShowBenchedPokemonActions);
                cardView.SetPlayable(true, clickBehaviour);
            }
        }

        private void HandleRetreat(GameInteraction interaction)
        {
            var card = (interaction.Data[typeof(InteractionCard)] as InteractionCard).Card;
            _playableCards.Add(card);
            var cardView = CardViewRegistry.INSTANCE.Get(card);
            if (interaction.Data.TryGetValue(typeof(ConditionalTargetData), out var targetData))
            {
                var conditionalTargetData = targetData as ConditionalTargetData;
                cardView.AddRetreat(() =>
                {
                    OnInteract();
                    SetUpSelection(
                        conditionalTargetData.ConditionOnSelection,
                        conditionalTargetData.PossibleTargets,
                        interaction.GameControllerMethodWithTargets,
                        true
                    );
                });
            }
            else
            {
                cardView.AddRetreat(() =>
                {
                    OnInteract();
                    interaction.GameControllerMethod.Invoke();
                });
            }
            var clickBehaviour = new ClickBehaviour(cardView.ShowActivePokemonActions);
            cardView.SetPlayable(true, clickBehaviour);
        }

        private void SetUpSelection(
            Predicate<List<ICard>> conditionOnSelection,
            List<ICard> possibleTargets,
            Action<List<ICard>> gameControllerMethodWithTargets,
            bool isQuickSelection
        )
        {
            ClearSelectedCards();
            Button button = null;
            if (!isQuickSelection)
            {
                button = _gameManager.EnableDoneButton(
                    () =>
                    {
                        gameControllerMethodWithTargets.Invoke(_selectedCards);
                        ClearSelectedCards();
                    },
                    () =>
                    {
                        OnInteract();
                        _gameManager.DisableSearchView();
                    }
                );
                button.interactable = conditionOnSelection(_selectedCards);
            }

            foreach (var possibleTarget in possibleTargets)
            {
                _playableCards.Add(possibleTarget);
                var cardView = CardViewRegistry.INSTANCE.Get(possibleTarget);
                cardView.SetPlayable(
                    true,
                    new ClickBehaviour(
                        () =>
                            HandleSelectionClicked(
                                cardView,
                                conditionOnSelection,
                                gameControllerMethodWithTargets,
                                button
                            )
                    )
                );
            }
        }

        private void HandleSelectionClicked(
            CardView cardView,
            Predicate<List<ICard>> conditionOnSelection,
            Action<List<ICard>> gameControllerMethodWithTargets,
            Button confirmButton
        )
        {
            if (cardView.Selected)
            {
                UnselectCardView(cardView);
                UpdateConfirmButton(conditionOnSelection, confirmButton);
                return;
            }
            SelectCardView(cardView);

            if (confirmButton != null)
            {
                if (confirmButton.interactable && !conditionOnSelection(_selectedCards))
                    UnselectCardView(cardView);
                confirmButton.interactable = conditionOnSelection(_selectedCards);
            }
            else
            {
                if (conditionOnSelection(_selectedCards))
                {
                    OnInteract();
                    gameControllerMethodWithTargets.Invoke(_selectedCards);
                    ClearSelectedCards();
                }
            }
        }

        private void UpdateConfirmButton(
            Predicate<List<ICard>> conditionOnSelection,
            Button confirmButton
        )
        {
            if (confirmButton != null)
            {
                confirmButton.interactable = conditionOnSelection(_selectedCards);
            }
        }

        private void SelectCardView(CardView cardView)
        {
            cardView.Selected = true;
            _selectedCards.Add(cardView.Card);
        }

        private void UnselectCardView(CardView cardView)
        {
            cardView.Selected = false;
            _selectedCards.Remove(cardView.Card);
        }

        private void HandleSelectActivePokemon(GameInteraction interaction)
        {
            var card = (interaction.Data[typeof(InteractionCard)] as InteractionCard).Card;
            _playableCards.Add(card);
            var cardView = CardViewRegistry.INSTANCE.Get(card);
            var clickBehaviour = new ClickBehaviour(() =>
            {
                OnInteract();
                _gameManager.PlayerActiveSpots[card.Owner].SetActivePokemon(card);

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
            _gameManager.AddOptionToMulliganSelector(
                (interaction.Data[typeof(NumberData)] as NumberData).Number,
                () =>
                {
                    OnInteract();
                    interaction.GameControllerMethod.Invoke();
                }
            );
        }

        private void OnInteract()
        {
            _gameManager.DisableButton();
            _gameManager.DisableFloatingSelection();
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

        private void ClearSelectedCards()
        {
            foreach (var cardView in CardViewRegistry.INSTANCE.GetAllAvailable(_selectedCards))
            {
                cardView.Selected = false;
            }
            _selectedCards.Clear();
        }
    }
}
