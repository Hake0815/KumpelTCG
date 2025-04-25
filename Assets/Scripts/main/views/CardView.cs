using System;
using DG.Tweening;
using gamecore.card;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace gameview
{
    public class CardView : MonoBehaviour
    {
        private const float ATTACHED_SCALE = 0.2f;

        [SerializeField]
        private AttackView _attackViewPrefab;
        private AttackView _currentAttackView;

        [SerializeField]
        private Image _damageIcon;

        [SerializeField]
        private TMP_Text _damageText;

        [SerializeField]
        private Sprite _backSideSprite;
        private Sprite _frontSideSprite;
        private Sprite _attachedSprite;
        private Image _image;
        private protected Collider2D _col;
        private protected Vector3 _positionBeforeDrag;
        private protected Transform _discardPilePosition;
        public RectTransform RectTransform { get; set; }

        public ICard Card { get; private set; }
        public Canvas Canvas { get; private set; }
        private bool _attached = false;
        public bool Attached
        {
            get => _attached;
            set
            {
                _attached = value;
                SetImageSprite();
            }
        }
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
            if (Attached && _attachedSprite != null)
                _image.sprite = _attachedSprite;
            else if (FaceUp)
                _image.sprite = _frontSideSprite;
            else
                _image.sprite = _backSideSprite;
        }

        private CardViewBehaviour _cardViewBehaviour;

        public void SetUp(Transform discardPilePosition, ICard card, Sprite frontSideSprite)
        {
            SetUp(discardPilePosition, card, frontSideSprite, null);
        }

        public void SetUp(
            Transform discardPilePosition,
            ICard card,
            Sprite frontSideSprite,
            Sprite attached
        )
        {
            _frontSideSprite = frontSideSprite;
            _discardPilePosition = discardPilePosition;
            _col = GetComponent<Collider2D>();
            _attachedSprite = attached;
            Card = card;
            OnEnable();
            SetImageSprite();
        }

        public void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            _image = GetComponentInChildren<Image>();
            Canvas = GetComponent<Canvas>();
        }

        private void OnEnable()
        {
            if (Card != null)
            {
                Card.CardDiscarded += Discard;
                if (Card is IPokemonCard pokemonCard)
                {
                    pokemonCard.EnergyAttached += AttachEnergy;
                    pokemonCard.DamageModified += UpdateDamage;
                }
            }
        }

        private void OnDisable()
        {
            if (Card != null)
            {
                Card.CardDiscarded -= Discard;
                if (Card is IPokemonCard pokemonCard)
                    pokemonCard.EnergyAttached -= AttachEnergy;
            }
        }

        private void Discard()
        {
            CardViewRegistry.INSTANCE.Unregister(Card);
            MoveToDiscardPile();
        }

        private void AttachEnergy(IEnergyCard card)
        {
            var energyCardView = CardViewRegistry.INSTANCE.Get(card);
            energyCardView.transform.DOScaleX(ATTACHED_SCALE, 0.25f);
            energyCardView.transform.DOScaleY(ATTACHED_SCALE / 1.375f, 0.25f);
            energyCardView.transform.DORotateQuaternion(transform.rotation, 0.25f);
            energyCardView.Attached = true;
            UpdateAttachedEnergyCards();
        }

        private void UpdateAttachedEnergyCards()
        {
            var height = RectTransform.rect.height;
            var width = RectTransform.rect.width;
            var verticalDirection = transform.rotation * Vector3.up;
            var horizontalDirection = transform.rotation * Vector3.right;
            var firstCardPosition =
                transform.position
                - height * (1 - ATTACHED_SCALE / 1.375f) / 2 * verticalDirection
                - width * (1 - ATTACHED_SCALE) / 2 * horizontalDirection;
            int i = 0;
            foreach (var energyCard in ((IPokemonCard)Card).AttachedEnergyCards)
            {
                var energyCardView = CardViewRegistry.INSTANCE.Get(energyCard);
                energyCardView.RectTransform.SetParent(transform);
                energyCardView.transform.DOMove(
                    firstCardPosition + i * width * ATTACHED_SCALE * horizontalDirection,
                    0.25f
                );
                i++;
            }
        }

        private void UpdateDamage()
        {
            var currentDamage = ((IPokemonCard)Card).Damage;
            if (currentDamage == 0)
                _damageIcon.gameObject.SetActive(false);
            else
            {
                _damageIcon.gameObject.SetActive(true);
                _damageText.text = currentDamage.ToString();
            }
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
                _cardViewBehaviour = cardViewBehaviour;
            }
            else
            {
                _cardViewBehaviour = null;
                _currentAttackView?.DestroyThis();
                _currentAttackView = null;
            }
        }

        public void AddAttack(IAttack attack, Action onAttackAction)
        {
            if (_currentAttackView == null)
            {
                var horizontalDirection = transform.rotation * Vector3.right;
                var distance =
                    (
                        _attackViewPrefab
                            .AttackButtonPrefab.GetComponent<RectTransform>()
                            .rect.width + RectTransform.rect.width
                    )
                    / 2f
                    * 1.1f
                    * horizontalDirection;
                _currentAttackView = Instantiate(
                    _attackViewPrefab,
                    transform.position + distance,
                    transform.rotation
                );
                _currentAttackView.Collider.Add(_col);
            }
            _currentAttackView.AddInteraction(attack, onAttackAction);
        }

        public void ShowAttacks()
        {
            _currentAttackView.Canvas.enabled = true;
        }
    }
}
