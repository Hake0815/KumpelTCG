using System;
using System.Collections.Generic;
using DG.Tweening;
using Mono.Cecil;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class HandView : MonoBehaviour
{
    private DeckView deck;
    private SplineContainer splineContainer;
    private CardView cardPrefab;

    private List<CardView> handCards = new();

    public void SetUp(DeckView deck, CardView cardPrefab)
    {
        this.deck = deck;
        this.cardPrefab = cardPrefab;
        splineContainer = GetComponent<SplineContainer>();
    }

    public void Register(IPlayer player)
    {
        player.CardDrawn += HandleCardDrown;
    }

    public void HandleCardDrown(object player, List<ICard> drawnCards)
    {
        foreach (var card in drawnCards)
        {
            var newCard = Instantiate(cardPrefab, deck.transform.position, deck.transform.rotation);
            handCards.Add(newCard);
            UpdateCardPosition();
        }
    }

    private void UpdateCardPosition()
    {
        if (handCards.Count == 0)
            return;
        var spacing = 1f / handCards.Count;
        var firstCardPosition = 0.5f - (handCards.Count - 1) * spacing / 2;
        var spline = splineContainer.Spline;
        for (int i = 0; i < handCards.Count; i++)
        {
            var p = firstCardPosition + i * spacing;
            var splinePosition = spline.EvaluatePosition(p);
            var forward = spline.EvaluateTangent(p);
            var up = spline.EvaluateUpVector(p);
            var rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
            handCards[i].transform.DOMove(splinePosition, 0.25f);
            handCards[i].transform.DOLocalRotateQuaternion(rotation, 0.25f);
        }
    }
}
