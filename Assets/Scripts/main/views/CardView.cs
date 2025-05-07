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
        private ActivePokemonActionsView _activePokemonActionsViewPrefab;
        private ActivePokemonActionsView _currentActivePokemonActionsView;

        [SerializeField]
        private Image _damageIcon;

        [SerializeField]
        private TMP_Text _damageText;

        [SerializeField]
        private Sprite _backSideSprite;
        private Sprite _frontSideSprite;
        private Sprite _attachedSprite;
        private Image _image;
        private Material _imageMaterial;
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

        private bool _playable;
        private bool _selected = false;
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (value)
                    TurnOnHighlight(Color.yellow);
                else if (_playable)
                    TurnOnHighlight(Color.green);
                else
                    TurnOffHighlight();
            }
        }

        private void SetImageSprite()
        {
            if (Attached && _attachedSprite != null)
            {
                _image.sprite = _attachedSprite;
                _imageMaterial.SetFloat("_Thickness", .1f);
            }
            else if (FaceUp)
            {
                _image.sprite = _frontSideSprite;
                _imageMaterial.SetFloat("_Thickness", .02f);
            }
            else
            {
                _image.sprite = _backSideSprite;
                _imageMaterial.SetFloat("_Thickness", .02f);
            }
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
            _imageMaterial = new Material(_image.material);
            _image.material = _imageMaterial;
            TurnOffHighlight();
        }

        private void OnEnable()
        {
            if (Card != null)
            {
                Card.CardDiscarded += Discard;
                if (Card is IPokemonCard pokemonCard)
                {
                    pokemonCard.OnAttachedEnergyChanged += AttachEnergy;
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
                    pokemonCard.OnAttachedEnergyChanged -= AttachEnergy;
            }
        }

        private void Discard()
        {
            CardViewRegistry.INSTANCE.Unregister(Card);
            MoveToDiscardPile();
        }

        private void AttachEnergy(IEnergyCard card)
        {
            UIQueue.INSTANCE.Queue(
                (callback) =>
                {
                    var sequence = DOTween.Sequence();
                    if (card != null)
                        CardViewRegistry.INSTANCE.Get(card).TransformToAttachedEnergyView(sequence);
                    UpdateAttachedEnergyCards(sequence);
                    sequence.OnComplete(() => callback.Invoke());
                }
            );
        }

        public void TransformToAttachedEnergyView(Sequence sequence)
        {
            sequence
                .Join(transform.DOScaleX(ATTACHED_SCALE, 0.25f))
                .Join(transform.DOScaleY(ATTACHED_SCALE / 1.375f, 0.25f))
                .Join(transform.DORotateQuaternion(transform.rotation, 0.25f));
            Attached = true;
        }

        private void UpdateAttachedEnergyCards(Sequence sequence)
        {
            var height = RectTransform.rect.height;
            var width = RectTransform.rect.width;
            var verticalDirection = transform.rotation * Vector3.up;
            var horizontalDirection = transform.rotation * Vector3.right;
            var firstCardPosition =
                transform.position
                - height * (1 - ATTACHED_SCALE / 1.375f) / 2 * verticalDirection
                - width * (1 - ATTACHED_SCALE) / 2 * horizontalDirection
                + Vector3.back;
            int i = 0;
            foreach (var energyCard in ((IPokemonCard)Card).AttachedEnergyCards)
            {
                var energyCardView = CardViewRegistry.INSTANCE.Get(energyCard);
                energyCardView.RectTransform.SetParent(transform);
                sequence.Join(
                    energyCardView.transform.DOMove(
                        firstCardPosition + i * width * ATTACHED_SCALE * horizontalDirection,
                        0.25f
                    )
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
            _playable = isPlayable;
            if (isPlayable)
            {
                _cardViewBehaviour = cardViewBehaviour;
                TurnOnHighlight(Color.green);
            }
            else
            {
                _cardViewBehaviour = null;
                _currentActivePokemonActionsView?.DestroyThis();
                _currentActivePokemonActionsView = null;
                TurnOffHighlight();
            }
        }

        private void TurnOnHighlight(Color color)
        {
            Debug.Log($"Turn on highlight with color {color}");
            _imageMaterial.SetColor("_Color", color);
            _imageMaterial.SetFloat("_Brightness", 5f);
        }

        private void TurnOffHighlight()
        {
            Debug.Log("Turn off highlight");
            _imageMaterial.SetColor("_Color", Color.white);
            _imageMaterial.SetFloat("_Brightness", 0f);
        }

        public void AddAttack(IAttack attack, Action onAttackAction)
        {
            if (_currentActivePokemonActionsView == null)
            {
                CreateCurrentActivePokemonActionsView();
            }
            _currentActivePokemonActionsView.AddAttackInteraction(attack, onAttackAction);
        }

        public void AddRetreat(Action onRetreatAction)
        {
            if (_currentActivePokemonActionsView == null)
                CreateCurrentActivePokemonActionsView();

            _currentActivePokemonActionsView.AddRetreatInteraction(
                (Card as IPokemonCard).RetreatCost,
                onRetreatAction
            );
        }

        private void CreateCurrentActivePokemonActionsView()
        {
            _currentActivePokemonActionsView = Instantiate(_activePokemonActionsViewPrefab);
            _currentActivePokemonActionsView.Collider.Add(_col);
        }

        public void ShowActivePokemonActions()
        {
            Vector3 distance = GetDistance();
            _currentActivePokemonActionsView.transform.position = transform.position + distance;
            _currentActivePokemonActionsView.transform.rotation = transform.rotation;
            _currentActivePokemonActionsView.Canvas.enabled = true;
        }

        private Vector3 GetDistance()
        {
            var horizontalDirection = transform.rotation * Vector3.right;
            var distance =
                (
                    _activePokemonActionsViewPrefab
                        .AttackButtonPrefab.GetComponent<RectTransform>()
                        .rect.width + RectTransform.rect.width
                )
                / 2f
                * 1.1f
                * horizontalDirection;
            return distance;
        }
    }
}
