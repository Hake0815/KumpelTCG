using System;
using DG.Tweening;
using gamecore.card;
using UnityEngine;

namespace gameview
{
    public class CardView : MonoBehaviour
    {
        private Collider2D col;
        private Vector3 positionBeforeDrag;
        private Transform discardPilePosition;
        public ICard Card { get; private set; }

        public void SetUp(Transform discardPilePosition, ICard card)
        {
            this.discardPilePosition = discardPilePosition;
            col = GetComponent<Collider2D>();
            Card = card;
            Card.CardDiscarded += Discard;
        }

        private void Discard()
        {
            transform.DOMove(discardPilePosition.position, 0.25f);
            transform.DORotateQuaternion(discardPilePosition.rotation, 0.25f);
        }

        private void OnMouseDown()
        {
            positionBeforeDrag = transform.position;
            transform.position = GetMousePosition();
        }

        private void OnMouseDrag()
        {
            transform.position = GetMousePosition();
        }

        private Vector3 GetMousePosition()
        {
            var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.z = 0f;
            return p;
        }

        private void OnMouseUp()
        {
            col.enabled = false;
            var hitCollider = Physics2D.OverlapPoint(transform.position);
            col.enabled = true;
            if (hitCollider != null && hitCollider.TryGetComponent(out ICardDropArea cardDropArea))
            {
                cardDropArea.OnCardDropped(this);
            }
            else
            {
                transform.position = positionBeforeDrag;
            }
        }
    }
}
