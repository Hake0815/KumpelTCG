using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

namespace gameview
{
    public interface ISplineCardHolder
    {
        SplineContainer SplineContainer { get; }

        public void UpdateCardPosition(List<CardView> cardViews, Quaternion parentRotation)
        {
            UIQueue.INSTANCE.Queue(CallbackOnDone =>
            {
                if (cardViews.Count == 0)
                    return;
                var spacing = Math.Min(1f / cardViews.Count, 0.05f);
                var firstCardPosition = -(cardViews.Count - 1) * spacing / 2 + 0.5f;
                var spline = SplineContainer.Spline;
                for (int i = 0; i < cardViews.Count; i++)
                {
                    cardViews[i].Canvas.sortingOrder = i;
                    var p = firstCardPosition + i * spacing;
                    var splinePosition = parentRotation * spline.EvaluatePosition(p);
                    var forward = spline.EvaluateTangent(p);
                    var up = spline.EvaluateUpVector(p);
                    var rotation =
                        parentRotation // spline.EvaluatePosition(p) seems to disregard the rotation of the spline container
                        * Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
                    cardViews[i].transform.DOMove(splinePosition + i * Vector3.back, 0.25f);
                    cardViews[i].transform.DOLocalRotateQuaternion(rotation, 0.25f);
                }
                CallbackOnDone.Invoke();
            });
        }
    }
}
