using System;
using DG.Tweening;
using gamecore.card;
using UnityEngine;
using UnityEngine.UI;

namespace gameview
{
    public class CardView : MonoBehaviour
    {
        [SerializeField]
        private Sprite _backSideSprite;
        private Sprite _frontSideSprite;
        private Image _image;
        private protected Collider2D _col;
        private protected Vector3 _positionBeforeDrag;
        private protected Transform _discardPilePosition;

        public ICard Card { get; private set; }
        public Canvas Canvas { get; private set; }
        private bool _faceUp = true;
        public bool FaceUp
        {
            get => _faceUp;
            set
            {
                _faceUp = value;
                SetImageSprite();
            }
        }

        private void SetImageSprite()
        {
            if (FaceUp)
                _image.sprite = _frontSideSprite;
            else
                _image.sprite = _backSideSprite;
        }

        private CardViewBehaviour _cardViewBehaviour;

        public void SetUp(Transform discardPilePosition, ICard card, Sprite frontSideSprite)
        {
            _frontSideSprite = frontSideSprite;
            _discardPilePosition = discardPilePosition;
            _col = GetComponent<Collider2D>();
            _col.enabled = false;
            Card = card;
            OnEnable();
            SetImageSprite();
        }

        public void Awake()
        {
            _image = GetComponentInChildren<Image>();
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
