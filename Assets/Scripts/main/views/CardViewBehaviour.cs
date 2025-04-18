using System;
using System.Collections.Generic;
using UnityEngine;

namespace gameview
{
    public abstract class CardViewBehaviour
    {
        public abstract void OnMouseDown(CardView cardView);
        public abstract void OnMouseDrag(CardView cardView);
        public abstract void OnMouseUp(Collider2D _col, CardView cardView);
    }

    public class ClickBehaviour : CardViewBehaviour
    {
        private readonly Action _onPlayed;

        public ClickBehaviour(Action onPlayed)
        {
            _onPlayed = onPlayed;
        }

        public override void OnMouseDown(CardView cardView)
        {
            _onPlayed?.Invoke();
        }

        public override void OnMouseDrag(CardView cardView) { }

        public override void OnMouseUp(Collider2D _col, CardView cardView) { }
    }

    public class DragBehaviour : CardViewBehaviour
    {
        private readonly Action _onPlayed;
        private protected Vector3 _positionBeforeDrag;
        private protected int _orderBeforeDrag;

        public DragBehaviour(Action onPlayed)
        {
            _onPlayed = onPlayed;
        }

        public override void OnMouseDown(CardView cardView)
        {
            _positionBeforeDrag = cardView.transform.position;
            _orderBeforeDrag = cardView.Canvas.sortingOrder;
            cardView.transform.position = GetMousePosition();
        }

        public override void OnMouseDrag(CardView cardView)
        {
            cardView.Canvas.sortingOrder = 99;
            cardView.transform.position = GetMousePosition();
        }

        private Vector3 GetMousePosition()
        {
            var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.z = 0f;
            return p;
        }

        public override void OnMouseUp(Collider2D _col, CardView cardView)
        {
            _col.enabled = false;
            var hitCollider = Physics2D.OverlapPoint(cardView.transform.position);
            _col.enabled = true;
            if (
                hitCollider != null
                && hitCollider.TryGetComponent(out ICardDropArea cardDropArea)
                && cardDropArea.OnCardDropped(cardView)
            )
            {
                cardView.Canvas.sortingOrder = -1;
                _onPlayed?.Invoke();
                return;
            }
            cardView.transform.position = _positionBeforeDrag;
            cardView.Canvas.sortingOrder = _orderBeforeDrag;
        }
    }

    public class DragToTargetBehaviour : DragBehaviour
    {
        private Action<List<object>> _onPlayed;
        private List<CardView> _targets;

        public DragToTargetBehaviour(Action<List<object>> onPlayed, List<CardView> targets)
            : base(null)
        {
            _onPlayed = onPlayed;
            _targets = targets;
        }

        public override void OnMouseUp(Collider2D col, CardView cardView)
        {
            col.enabled = false;
            var hitCollider = Physics2D.OverlapPoint(cardView.transform.position);
            col.enabled = true;
            if (
                hitCollider != null
                && hitCollider.TryGetComponent(out CardView targetCardView)
                && _targets.Contains(targetCardView)
            )
            {
                cardView.Canvas.sortingOrder = targetCardView.Canvas.sortingOrder + 1;
                _onPlayed?.Invoke(new() { targetCardView.Card });
                return;
            }
            cardView.transform.position = _positionBeforeDrag;
            cardView.Canvas.sortingOrder = _orderBeforeDrag;
        }
    }
}
