using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        [SerializeField]
        private MulliganSelectorView _mulliganSelectorViewPrefab;
        private readonly Dictionary<IPlayer, HandView> _playerHandViews = new();
        public Dictionary<IPlayer, ActiveSpot> PlayerActiveSpots { get; } = new();

        public Button endTurnButton;

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
            DisablePlayerHandViews();
            gameRemoteService.StartGame();
        }

        public void EnablePlayerHandViews()
        {
            foreach (var handView in _playerHandViews.Values)
            {
                handView.gameObject.SetActive(true);
            }
        }

        public void DisablePlayerHandViews()
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

        public void ShowMulligan(IPlayer player, List<List<ICard>> mulligans, Action onDone)
        {
            if (mulligans.Count == 0)
            {
                onDone();
                return;
            }
            UIQueue.INSTANCE.Queue(
                (OnUICompleted) => CreateMulliganView(player, mulligans, onDone, OnUICompleted)
            );
        }

        private void CreateMulliganView(
            IPlayer player,
            List<List<ICard>> mulligans,
            Action onDone,
            Action OnUICompleted
        )
        {
            var mulliganView = Instantiate(_mulliganViewPrefab);
            mulliganView.SetUp(player, mulligans);
            mulliganView.AddDoneListener(() =>
            {
                onDone();
                OnUICompleted();
            });
        }

        public void ShowGameState()
        {
            foreach (var player in _playerHandViews)
            {
                player.Value.CreateHandCards();
            }
        }

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
            endTurnButton.onClick.RemoveAllListeners();
            endTurnButton.gameObject.SetActive(false);
        }

        internal void ShowMulliganSelector(List<object> possibleTargets, Action<object> onConfirm)
        {
            var mulliganSelectorView = Instantiate(_mulliganSelectorViewPrefab);
            mulliganSelectorView.SetUp(possibleTargets, onConfirm);
        }
    }
}
