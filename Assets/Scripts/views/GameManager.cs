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
        private Game game;

        [SerializeField]
        private HandView _handView;

        [SerializeField]
        private DeckView _deckView;

        [SerializeField]
        private PlayArea _playArea;

        [SerializeField]
        private CardViewCreator _cardViewCreator;

        [SerializeField]
        private DiscardPileView _discardPileView;

        private readonly Dictionary<IPlayer, HandView> _playerHandViews = new();

        public Button endTurnButton;

        void Start()
        {
            endTurnButton = GetComponentInChildren<Button>();
            endTurnButton.onClick.AddListener(EndTurn);
            Instantiate(_cardViewCreator);

            InitializeGame();
            game.PerformSetup();

            SetUpPlayerViews(game.Player1, new Quaternion(0f, 0f, 0f, 1f));
            SetUpPlayerViews(game.Player2, new Quaternion(0f, 0f, 1f, 0f));
            ShowGameState();

            game.StartGame();
        }

        private void InitializeGame()
        {
            game = new();
            List<ICard> cardsPlayer1 = CreateCardList(game.Player1);
            List<ICard> cardsPlayer2 = CreateCardList(game.Player2);
            game.Initialize(cardsPlayer1, cardsPlayer2);
        }

        private void SetUpPlayerViews(IPlayer player, Quaternion rotation)
        {
            var discardPileView = Instantiate(
                _discardPileView,
                rotation * _discardPileView.transform.position,
                rotation
            );
            CardViewCreator.INSTANCE.DiscardPileViews.Add(player, discardPileView);
            var handView = Instantiate(_handView, _handView.transform.position, rotation); // Position is at 0,0,0
            var deckView = Instantiate(
                _deckView,
                rotation * _deckView.transform.position,
                rotation
            );
            var playArea = Instantiate(
                _playArea,
                rotation * _playArea.transform.position,
                rotation
            );
            playArea.SetUp(player);
            handView.SetUp(deckView);
            deckView.SetUp(player);
            handView.Register(player);
            discardPileView.SetUp(player.DiscardPile);
            _playerHandViews.Add(player, handView);
        }

        private List<ICard> CreateCardList(IPlayer player)
        {
            var cards = new List<ICard>();
            cards.AddRange(CardFactory.CreateCard("bill", player, 59));
            cards.AddRange(CardFactory.CreateCard("TWM128", player, 1));
            return cards;
        }

        private void ShowGameState()
        {
            foreach (var player in _playerHandViews)
            {
                player.Value.CreateHandCards();
            }
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
