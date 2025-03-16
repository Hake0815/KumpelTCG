using System;
using System.Collections.Generic;
using UnityEngine;

public class CardViewCreator : MonoBehaviour
{
    public static CardViewCreator INSTANCE { get; private set; }

    protected virtual void Awake()
    {
        if (INSTANCE != null)
        {
            Destroy(gameObject);
            return;
        }
        INSTANCE = this;
    }

    protected virtual void OicationQuit()
    {
        INSTANCE = null;
        Destroy(gameObject);
    }

    [SerializeField]
    private CardView cardPrefab;

    public CardView CreateAt(ICard card, Vector3 position, Quaternion rotation)
    {
        var newCardView = Instantiate(cardPrefab, position, rotation);
        newCardView.Card = card;
        CardViewRegistry.INSTANCE.Register(newCardView);
        return newCardView;
    }
}
