using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Game game;

    [SerializeField]
    private HandView handView;

    [SerializeField]
    private DeckView deckView;

    [SerializeField]
    private CardView cardPrefab;

    void Start()
    {
        game = new();
        List<ICard> cardsPlayer1 = new()
        {
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
        };
        List<ICard> cardsPlayer2 = new()
        {
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
            CardDummy.Of(CardDatabase.cardDataList[0]),
        };

        game.SetUp(cardsPlayer1, cardsPlayer2);
        handView = Instantiate(handView);
        deckView = Instantiate(deckView);
        handView.SetUp(deckView, cardPrefab);
        handView.Register(game.Player1);
        game.StartGame();
    }
}
