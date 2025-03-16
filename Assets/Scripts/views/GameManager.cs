using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private Game game;

    [SerializeField]
    private HandView handView;

    [SerializeField]
    private DeckView deckView;

    [SerializeField]
    private CardViewCreator cardViewCreator;

    public Button endTurnButton;

    private HandView handViewPlayer1;
    private DeckView deckViewPlayer1;
    private HandView handViewPlayer2;
    private DeckView deckViewPlayer2;

    void Start()
    {
        endTurnButton = GetComponentInChildren<Button>();
        endTurnButton.onClick.AddListener(EndTurn);
        Instantiate(cardViewCreator);

        SetUpGame();
        SetUpPlayer1Views();
        SetUpPlayer2Views();

        game.StartGame();
    }

    private void SetUpPlayer2Views()
    {
        var rotation = new Quaternion(0f, 0f, 1f, 0f);
        handViewPlayer2 = Instantiate(handView, handView.transform.position, rotation);
        deckViewPlayer2 = Instantiate(deckView, rotation * deckView.transform.position, rotation);
        handViewPlayer2.SetUp(deckViewPlayer2);
        deckViewPlayer2.SetUp(game.Player2);
        deckViewPlayer2.Text.transform.rotation =
            rotation * deckViewPlayer2.Text.transform.rotation;
        handViewPlayer2.Register(game.Player2);
    }

    private void SetUpPlayer1Views()
    {
        handViewPlayer1 = Instantiate(handView);
        deckViewPlayer1 = Instantiate(deckView);
        handViewPlayer1.SetUp(deckViewPlayer1);
        deckViewPlayer1.SetUp(game.Player1);
        handViewPlayer1.Register(game.Player1);
    }

    private void SetUpGame()
    {
        game = new();
        List<ICard> cardsPlayer1 = CreateCardList();
        List<ICard> cardsPlayer2 = CreateCardList();
        game.SetUp(cardsPlayer1, cardsPlayer2);
    }

    private List<ICard> CreateCardList()
    {
        return new()
        {
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
        };
    }

    public void EndTurn()
    {
        game.EndTurn();
    }
}
