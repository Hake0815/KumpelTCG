using System;
using UnityEngine;

namespace gameview
{
    public abstract class CardViewBehaviour
    {
        private protected readonly Action _onPlayed;

        public CardViewBehaviour(Action onPlayed)
        {
            _onPlayed = onPlayed;
        }

        public abstract void OnMouseDown(Transform transform);
        public abstract void OnMouseDrag(Transform transform);
        public abstract void OnMouseUp(Collider2D _col, Transform transform, CardView cardView);
    }

    public class ClickBehaviour : CardViewBehaviour
    {
        public ClickBehaviour(Action onPlayed)
            : base(onPlayed) { }

        public override void OnMouseDown(Transform transform)
        {
            _onPlayed?.Invoke();
        }

        public override void OnMouseDrag(Transform transform) { }

        public override void OnMouseUp(Collider2D _col, Transform transform, CardView cardView) { }
    }

    public class DragBehaviour : CardViewBehaviour
    {
        private protected Vector3 _positionBeforeDrag;

        public DragBehaviour(Action onPlayed)
            : base(onPlayed) { }

        public override void OnMouseDown(Transform transform)
        {
            _positionBeforeDrag = transform.position;
            transform.position = GetMousePosition();
        }

        public override void OnMouseDrag(Transform transform)
        {
            transform.position = GetMousePosition();
        }

        private Vector3 GetMousePosition()
        {
            var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.z = 0f;
            return p;
        }

        public override void OnMouseUp(Collider2D _col, Transform transform, CardView cardView)
        {
            _col.enabled = false;
            var hitCollider = Physics2D.OverlapPoint(transform.position);
            _col.enabled = true;
            if (hitCollider != null && hitCollider.TryGetComponent(out ICardDropArea cardDropArea))
            {
                if (cardDropArea.OnCardDropped(cardView))
                {
                    _onPlayed?.Invoke();
                    return;
                }
            }
            transform.position = _positionBeforeDrag;
        }
    }
}
