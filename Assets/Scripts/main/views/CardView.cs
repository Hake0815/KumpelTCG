using System;
using DG.Tweening;
using gamecore.card;
using UnityEngine;
using UnityEngine.UI;

namespace gameview
{
    public class CardView : MonoBehaviour
    {
        private protected Collider2D _col;
        private protected Vector3 _positionBeforeDrag;
        private protected Transform _discardPilePosition;
        public ICard Card { get; private set; }
        public Image Image { get; set; }
        public Canvas Canvas { get; private set; }
        private CardViewBehaviour _cardViewBehaviour;

        public void SetUp(Transform discardPilePosition, ICard card)
        {
            _discardPilePosition = discardPilePosition;
            _col = GetComponent<Collider2D>();
            _col.enabled = false;
            Card = card;
            OnEnable();
        }

        public void Awake()
        {
            Image = GetComponentInChildren<Image>();
            Canvas = GetComponent<Canvas>();
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
                .Append(transform.DOMove(_discardPilePosition.position, 0.25f))
                .Join(transform.DORotateQuaternion(_discardPilePosition.rotation, 0.25f))
                .OnComplete(() => Destroy(gameObject))
                .WaitForCompletion();
        }

        private void OnMouseDown()
        {
            _cardViewBehaviour?.OnMouseDown(this);
        }

        private void OnMouseDrag()
        {
            _cardViewBehaviour?.OnMouseDrag(this);
        }

        private void OnMouseUp()
        {
            _cardViewBehaviour?.OnMouseUp(_col, this);
        }

        internal void SetPlayable(bool isPlayable, CardViewBehaviour cardViewBehaviour)
        {
            if (isPlayable)
            {
                _col.enabled = true;
                _cardViewBehaviour = cardViewBehaviour;
            }
            else
            {
                _col.enabled = false;
                _cardViewBehaviour = null;
            }
        }
    }
}
