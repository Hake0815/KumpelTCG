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
        private readonly Dictionary<IPlayer, ActiveSpot> _playerActiveSpots = new();

        public Button endTurnButton;
        public GameManagerState GameManagerState { get; private set; }

        void Start()
        {
            endTurnButton = GetComponentInChildren<Button>();
            endTurnButton.gameObject.SetActive(false);
            endTurnButton.onClick.AddListener(EndTurn);
            Instantiate(_cardViewCreator);

            InitializeGame();

            SetUpPlayerViews(_gameController.Game.Player1, new Quaternion(0f, 0f, 0f, 1f));
            SetUpPlayerViews(_gameController.Game.Player2, new Quaternion(0f, 0f, 1f, 0f));
            _gameController.PerformSetup();
            GameManagerState = GameManagerStateFactory.CreateMulliganStatePlayer1(this);
        }

        private void InitializeGame()
        {
            _gameController = new GameBuilder()
                .WithPlayer1Decklist(CreateDeckList())
                .WithPlayer2Decklist(CreateDeckList())
                .Build();
        }

        private Dictionary<string, int> CreateDeckList()
        {
            var decklist = new Dictionary<string, int>();
            decklist.Add("bill", 1);
            decklist.Add("TWM128", 4);
            return decklist;
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
            _playerActiveSpots.Add(player, activeSpot);
        }

        /*
         * Returns true if the mulligan was shown, false if the player has no mulligans
         */
        public bool ShowMulliganPlayer1()
        {
            return ShowMulligan(_gameController.Player1);
        }

        /*
         * Returns true if the mulligan was shown, false if the player has no mulligans
         */
        public bool ShowMulliganPlayer2()
        {
            return ShowMulligan(_gameController.Player2);
        }

        /*
         * Returns true if the mulligan was shown, false if the player has no mulligans
         */
        private bool ShowMulligan(IPlayer player)
        {
            var mulligans = _gameController.GameSetupBuilder.GetMulligansForPlayer(player);
            if (mulligans.Count == 0)
            {
                return false;
            }
            var mulliganView = Instantiate(_mulliganViewPrefab);
            mulliganView.SetUp(player, mulligans);
            mulliganView.AddDoneListener(() =>
            {
                GameManagerState = GameManagerState.AdvanceSuccessfully();
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

        public void WaitForActivePokemon()
        {
            foreach (var player in _playerActiveSpots)
            {
                Debug.Log($"Register on click listener for {player.Key.Name}: {player.Value}");
                player.Value.CardPlayed += () =>
                    GameManagerState = GameManagerState.AdvanceSuccessfully();
            }
        }

        public void StartGame()
        {
            _gameController.StartGame();
            endTurnButton.gameObject.SetActive(true);
        }

        public void EndTurn()
        {
            if (ActionSystem.INSTANCE.IsPerforming)
                return;
            var endTurn = new EndTurnGA();
            ActionSystem.INSTANCE.Perform(endTurn);
        }
    }
}
