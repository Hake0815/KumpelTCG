using System;
using UnityEngine;

namespace gameview
{
    public abstract class CardViewBehaviour
    {
        private protected readonly Action _onPlayed;

        protected CardViewBehaviour(Action onPlayed)
        {
            _onPlayed = onPlayed;
        }

        public abstract void OnMouseDown(CardView cardView);
        public abstract void OnMouseDrag(CardView cardView);
        public abstract void OnMouseUp(Collider2D _col, CardView cardView);
    }

    public class ClickBehaviour : CardViewBehaviour
    {
        public ClickBehaviour(Action onPlayed)
            : base(onPlayed) { }

        public override void OnMouseDown(CardView cardView)
        {
            _onPlayed?.Invoke();
        }

        public override void OnMouseDrag(CardView cardView) { }

        public override void OnMouseUp(Collider2D _col, CardView cardView) { }
    }

    public class DragBehaviour : CardViewBehaviour
    {
        private Vector3 _positionBeforeDrag;
        private int _orderBeforeDrag;

        public DragBehaviour(Action onPlayed)
            : base(onPlayed) { }

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
}
