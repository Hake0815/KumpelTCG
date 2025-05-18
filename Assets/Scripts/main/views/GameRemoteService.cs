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
                    case GameInteractionType.SelectCards:
                        HandleSelectCards(interaction);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private void HandleSelectCards(GameInteraction interaction)
        {
            var targetData = interaction.Data[typeof(TargetData)] as TargetData;
            switch ((interaction.Data[typeof(SelectFromData)] as SelectFromData).SelectFrom)
            {
                case SelectFrom.InPlay:
                    // Nothing to do here
                    break;
                case SelectFrom.Floating:
                    PrepareFloatingSelection(targetData.PossibleTargets);
                    break;
                default:
                    throw new NotImplementedException();
            }
            if (targetData.NumberOfTargets != 1)
                throw new NotImplementedException();

            foreach (var card in targetData.PossibleTargets)
            {
                _playableCards.Add(card);
                var cardView = CardViewRegistry.INSTANCE.Get(card);
                cardView.SetPlayable(
                    true,
                    new ClickBehaviour(() =>
                    {
                        OnInteract();
                        interaction.GameControllerMethodWithTargets.Invoke(new() { card });
                    })
                );
            }
        }

        private void PrepareFloatingSelection(List<ICard> possibleTargets)
        {
            _gameManager.ActivateFloatingSelectionView(possibleTargets);
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
                        interaction.GameControllerMethodWithTargets
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
            Action<List<ICard>> gameControllerMethodWithTargets
        )
        {
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
                                gameControllerMethodWithTargets
                            )
                    )
                );
            }
        }

        private void HandleSelectionClicked(
            CardView cardView,
            Predicate<List<ICard>> conditionOnSelection,
            Action<List<ICard>> gameControllerMethodWithTargets
        )
        {
            if (cardView.Selected)
            {
                cardView.Selected = false;
                _selectedCards.Remove(cardView.Card);
                return;
            }
            cardView.Selected = true;
            _selectedCards.Add(cardView.Card);
            if (conditionOnSelection(_selectedCards))
            {
                OnInteract();
                gameControllerMethodWithTargets.Invoke(_selectedCards);
                ClearSelectedCards();
            }
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

        private static Dictionary<string, int> CreateDeckList()
        {
            return new Dictionary<string, int>
            {
                { "bill", 16 },
                { "TWM128", 8 },
                { "TWM129", 8 },
                { "FireNRG", 14 },
                { "PsychicNRG", 14 },
            };
        }
    }
}
