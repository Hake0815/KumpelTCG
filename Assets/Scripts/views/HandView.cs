using System;
using System.Collections.Generic;
using DG.Tweening;
using gamecore.card;
using gamecore.game;
using UnityEngine;
using UnityEngine.Splines;

namespace gameview
{
    public class HandView : MonoBehaviour
    {
        private Transform deckPosition;
        private SplineContainer splineContainer;
        private IPlayer player;

        public void SetUp(DeckView deck)
        {
            deckPosition = deck.transform;
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
                    deckPosition.position,
                    deckPosition.rotation
                );
                UpdateCardPosition();
            }
        }

        private void UpdateCardPosition()
        {
            var handCards = CardViewRegistry.INSTANCE.GetAll(player.Hand);
            if (handCards.Count == 0)
                return;
            var spacing = Math.Min(1f / handCards.Count, 0.05f);
            var firstCardPosition = -(handCards.Count - 1) * spacing / 2 + 0.5f;
            var spline = splineContainer.Spline;
            for (int i = 0; i < handCards.Count; i++)
            {
                var p = firstCardPosition + i * spacing;
                var splinePosition = transform.rotation * spline.EvaluatePosition(p);
                var forward = spline.EvaluateTangent(p);
                var up = spline.EvaluateUpVector(p);
                var rotation =
                    transform.rotation // spline.EvaluatePosition(p) seems to disregard the rotation of the spline container
                    * Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
                handCards[i].transform.DOMove(splinePosition, 0.25f);
                handCards[i].transform.DOLocalRotateQuaternion(rotation, 0.25f);
            }
        }
    }
}
