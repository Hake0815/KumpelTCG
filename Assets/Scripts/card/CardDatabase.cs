using System.Collections.Generic;
using UnityEngine;

public class CardDatabase
{
    public static List<CardData> cardDataList = new()
    {
        new CardDataDummy("first Card"),
        new CardDataDummy("second Card"),
        new CardDataDummy("third Card"),
    };
}
