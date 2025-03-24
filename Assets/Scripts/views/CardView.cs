using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using gamecore.card;
using UnityEngine;
using UnityEngine.UI;

namespace gameview
{
    public class CardView : MonoBehaviour
    {
        private protected Collider2D col;
        private protected Vector3 positionBeforeDrag;
        private protected Transform discardPilePosition;
        public ICard Card { get; private set; }
        public Image Image { get; set; }

        public void SetUp(Transform discardPilePosition, ICard card)
        {
            this.discardPilePosition = discardPilePosition;
            col = GetComponent<Collider2D>();
            Card = card;
            OnEnable();
        }

        public void Awake()
        {
            Image = GetComponentInChildren<Image>();
        }

        private void OnEnable()
        {
            if (Card != null)
                Card.CardDiscarded += Discard;
        }

        private void OnDisable()
        {
            if (Card != null)
                Card.CardDiscarded -= Discard;
        }

        private void Discard()
        {
            CardViewRegistry.INSTANCE.Unregister(Card);
            MoveToDiscardPile();
        }

        private void MoveToDiscardPile()
        {
            DOTween
                .Sequence()
                .Append(transform.DOMove(discardPilePosition.position, 0.25f))
                .Join(transform.DORotateQuaternion(discardPilePosition.rotation, 0.25f))
                .OnComplete(() => Destroy(gameObject))
                .WaitForCompletion();
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
                if (cardDropArea.OnCardDropped(this))
                {
                    return;
                }
            }
            transform.position = positionBeforeDrag;
        }
    }
}
