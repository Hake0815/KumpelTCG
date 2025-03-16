using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

public class HandView : MonoBehaviour
{
    private DeckView deck;
    private SplineContainer splineContainer;
    private IPlayer player;

    public void SetUp(DeckView deck)
    {
        this.deck = deck;
        splineContainer = GetComponent<SplineContainer>();
    }

    public void Register(IPlayer player)
    {
        player.CardDrawn += HandleCardDrawn;
        this.player = player;
    }

    public void HandleCardDrawn(object player, List<ICard> drawnCards)
    {
        foreach (var card in drawnCards)
        {
            CardViewCreator.INSTANCE.CreateAt(
                card,
                deck.transform.position,
                deck.transform.rotation
            );
            UpdateCardPosition();
        }
    }

    private void UpdateCardPosition()
    {
        var handCards = CardViewRegistry.INSTANCE.GetAll(player.Hand);
        Debug.Log(handCards.Count + " Cards in Hand of " + player.Name);
        if (handCards.Count == 0)
            return;
        var spacing = Math.Min(1f / handCards.Count, 0.05f);
        var firstCardPosition = 0.5f - (handCards.Count - 1) * spacing / 2;
        var spline = splineContainer.Spline;
        for (int i = 0; i < handCards.Count; i++)
        {
            var p = firstCardPosition + i * spacing;
            var splinePosition = transform.rotation * spline.EvaluatePosition(p);
            Debug.Log(splinePosition);
            var forward = spline.EvaluateTangent(p);
            var up = spline.EvaluateUpVector(p);
            var rotation =
                transform.rotation
                * Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
            handCards[i].transform.DOMove(splinePosition, 0.25f);
            handCards[i].transform.DOLocalRotateQuaternion(rotation, 0.25f);
        }
    }
}
