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

        public Button endTurnButton;

        void Start()
        {
            endTurnButton = GetComponentInChildren<Button>();
            endTurnButton.onClick.AddListener(EndTurn);
            Instantiate(_cardViewCreator);

            SetUpGame();
            SetUpPlayerViews(game.Player1, new Quaternion(0f, 0f, 0f, 1f));
            SetUpPlayerViews(game.Player2, new Quaternion(0f, 0f, 1f, 0f));

            game.StartGame();
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
        }

        private void SetUpGame()
        {
            game = new();
            List<ICard> cardsPlayer1 = CreateCardList(game.Player1);
            List<ICard> cardsPlayer2 = CreateCardList(game.Player2);
            game.SetUp(cardsPlayer1, cardsPlayer2);
        }

        private List<ICard> CreateCardList(IPlayer player)
        {
            return new()
            {
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
                TrainerCard.Of(CardDatabase.cardDataList[0] as ITrainerCardData, player),
            };
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
