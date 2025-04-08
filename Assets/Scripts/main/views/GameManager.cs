using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game;
using gamecore.game.action;
using UnityEngine;
using UnityEngine.UI;

namespace gameview
{
    public class GameManager : MonoBehaviour
    {
        private IGameController _gameController;

        [SerializeField]
        private HandView _handView;

        [SerializeField]
        private DeckView _deckView;

        [SerializeField]
        private PlayArea _playArea;

        [SerializeField]
        private ActiveSpot _activeSpot;

        [SerializeField]
        private CardViewCreator _cardViewCreator;

        [SerializeField]
        private DiscardPileView _discardPileView;

        [SerializeField]
        private MulliganView _mulliganViewPrefab;

        private readonly Dictionary<IPlayer, HandView> _playerHandViews = new();
        public Dictionary<IPlayer, ActiveSpot> PlayerActiveSpots { get; } = new();

        public Button endTurnButton;

        // public GameManagerState GameManagerState { get; private set; }

        void Start()
        {
            endTurnButton = GetComponentInChildren<Button>();
            Instantiate(_cardViewCreator);

            var gameRemoteService = new GameRemoteService(this);

            SetUpPlayerViews(
                gameRemoteService.GameController.Game.Player1,
                new Quaternion(0f, 0f, 0f, 1f)
            );
            SetUpPlayerViews(
                gameRemoteService.GameController.Game.Player2,
                new Quaternion(0f, 0f, 1f, 0f)
            );
            ShowGameState();
            gameRemoteService.StartGame();
        }

        private void EnablePlayerHandViews()
        {
            foreach (var handView in _playerHandViews.Values)
            {
                handView.gameObject.SetActive(true);
            }
        }

        private void DisablePlayerHandViews()
        {
            foreach (var handView in _playerHandViews.Values)
            {
                handView.gameObject.SetActive(false);
            }
        }

        private void SetUpPlayerViews(IPlayer player, Quaternion rotation)
        {
            SetUpDiscardPileView(player, rotation);
            var deckView = SetUpDeckView(player, rotation);
            SetUpHandView(player, rotation, deckView);
            SetUpPlayArea(player, rotation);
            SetUpActiveSpot(player, rotation);
        }

        private void SetUpDiscardPileView(IPlayer player, Quaternion rotation)
        {
            var discardPileView = Instantiate(
                _discardPileView,
                rotation * _discardPileView.transform.position,
                rotation
            );
            discardPileView.SetUp(player.DiscardPile);
            CardViewCreator.INSTANCE.DiscardPileViews.Add(player, discardPileView);
        }

        private DeckView SetUpDeckView(IPlayer player, Quaternion rotation)
        {
            var deckView = Instantiate(
                _deckView,
                rotation * _deckView.transform.position,
                rotation
            );
            deckView.SetUp(player);
            return deckView;
        }

        private void SetUpHandView(IPlayer player, Quaternion rotation, DeckView deckView)
        {
            var handView = Instantiate(_handView, _handView.transform.position, rotation); // Position is at 0,0,0
            handView.Register(player);
            handView.SetUp(deckView);
            _playerHandViews.Add(player, handView);
        }

        private void SetUpPlayArea(IPlayer player, Quaternion rotation)
        {
            var playArea = Instantiate(
                _playArea,
                rotation * _playArea.transform.position,
                rotation
            );
            playArea.SetUp(player);
        }

        private void SetUpActiveSpot(IPlayer player, Quaternion rotation)
        {
            var activeSpot = Instantiate(
                _activeSpot,
                rotation * _activeSpot.transform.position,
                rotation
            );
            activeSpot.SetUp(player);
            PlayerActiveSpots.Add(player, activeSpot);
        }

        /*
         * Returns true if the mulligan was shown, false if the player has no mulligans
         */
        // public bool ShowMulliganPlayer1()
        // {
        //     // return ShowMulligan(_gameController.Player1);
        // }

        /*
         * Returns true if the mulligan was shown, false if the player has no mulligans
         */
        // public bool ShowMulliganPlayer2()
        // {
        //     return ShowMulligan(_gameController.Player2);
        // }

        /*
         * Returns true if the mulligan was shown, false if the player has no mulligans
         */
        public bool ShowMulligan(IPlayer player, List<List<ICard>> mulligans, Action onDone)
        {
            if (mulligans.Count == 0)
            {
                return false;
            }
            var mulliganView = Instantiate(_mulliganViewPrefab);
            mulliganView.SetUp(player, mulligans);
            mulliganView.AddDoneListener(() =>
            {
                onDone();
            });
            return true;
        }

        public void ShowGameState()
        {
            foreach (var player in _playerHandViews)
            {
                player.Value.CreateHandCards();
            }
        }

        // public void WaitForActivePokemon()
        // {
        //     foreach (var player in _playerActiveSpots)
        //     {
        //         Debug.Log($"Register on click listener for {player.Key.Name}: {player.Value}");
        //         player.Value.CardPlayed += () =>
        //             GameManagerState = GameManagerState.AdvanceSuccessfully();
        //     }
        // }

        // public void StartGame()
        // {
        //     _gameController.StartGame();
        // }

        public void EnableEndTurn(Action gameControllerMethod, Action onInteract)
        {
            endTurnButton.gameObject.SetActive(true);
            endTurnButton.onClick.AddListener(() =>
            {
                onInteract();
                gameControllerMethod();
            });
        }

        public void DisableEndTurn()
        {
            endTurnButton.gameObject.SetActive(false);
        }
    }
}
