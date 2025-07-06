using System;
using System.Collections.Generic;
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
        private BenchedPokemonActionsView _benchedPokemonActionsViewPrefab;
        private BenchedPokemonActionsView _currentBenchedPokemonActionsView;

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

        private float _height;
        private float _width;

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

        public void SetUp(ICard card, Sprite frontSideSprite)
        {
            SetUp(card, frontSideSprite, null);
        }

        public void SetUp(ICard card, Sprite frontSideSprite, Sprite attached)
        {
            _frontSideSprite = frontSideSprite;
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
            _height = RectTransform.rect.height;
            _width = RectTransform.rect.width;
        }

        private void OnEnable()
        {
            if (Card is IPokemonCard pokemonCard)
            {
                pokemonCard.OnAttachedEnergyChanged += AttachEnergy;
                pokemonCard.DamageModified += UpdateDamage;
                pokemonCard.Evolved += OnEvolved;
                UpdateDamage();
            }
        }

        private void OnDisable()
        {
            if (Card is IPokemonCard pokemonCard)
            {
                pokemonCard.OnAttachedEnergyChanged -= AttachEnergy;
                pokemonCard.DamageModified -= UpdateDamage;
                pokemonCard.Evolved -= OnEvolved;
            }
        }

        private void OnEvolved()
        {
            UIQueue.INSTANCE.Queue(
                (callback) =>
                {
                    Destroy(gameObject);
                    callback.Invoke();
                }
            );
        }

        public void AttachEnergy(List<IEnergyCard> cards)
        {
            UIQueue.INSTANCE.Queue(
                (callback) =>
                {
                    var sequence = DOTween.Sequence();
                    foreach (var card in cards)
                    {
                        if (cards != null)
                            CardViewRegistry
                                .INSTANCE.Get(card)
                                .TransformToAttachedEnergyView(sequence);
                    }
                    UpdateAttachedEnergyCards(sequence);
                    sequence.OnComplete(() => callback.Invoke());
                }
            );
        }

        private void TransformToAttachedEnergyView(Sequence sequence)
        {
            sequence
                .Join(transform.DOScaleX(ATTACHED_SCALE, AnimationSpeedHolder.AnimationSpeed))
                .Join(
                    transform.DOScaleY(ATTACHED_SCALE / 1.375f, AnimationSpeedHolder.AnimationSpeed)
                )
                .Join(
                    transform.DORotateQuaternion(
                        transform.rotation,
                        AnimationSpeedHolder.AnimationSpeed
                    )
                );
            Attached = true;
        }

        private void UpdateAttachedEnergyCards(Sequence sequence)
        {
            int i = 0;
            foreach (var energyCard in ((IPokemonCard)Card).AttachedEnergyCards)
            {
                var energyCardView = CardViewRegistry.INSTANCE.Get(energyCard);
                energyCardView.RectTransform.SetParent(transform);
                sequence.Join(
                    energyCardView.transform.DOLocalMove(
                        GetEnergyTargetPosition(i),
                        AnimationSpeedHolder.AnimationSpeed
                    )
                );
                i++;
            }
        }

        private Vector3 GetEnergyTargetPosition(int i)
        {
            return GetFirstLocalCardPosition() + i * _width * ATTACHED_SCALE * Vector3.right;
        }

        private Vector3 GetFirstLocalCardPosition()
        {
            return -_height * (1 - ATTACHED_SCALE / 1.375f) / 2 * Vector3.up
                - _width * (1 - ATTACHED_SCALE) / 2 * Vector3.right
                + Vector3.back;
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
                _currentBenchedPokemonActionsView?.DestroyThis();
                _currentBenchedPokemonActionsView = null;
                TurnOffHighlight();
            }
        }

        private void TurnOnHighlight(Color color)
        {
            _imageMaterial.SetColor("_Color", color);
            _imageMaterial.SetFloat("_Brightness", 5f);
        }

        private void TurnOffHighlight()
        {
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

        public void AddAbility(
            IAbility ability,
            Action onAbilityAction,
            bool isActivePokemon = true
        )
        {
            if (isActivePokemon)
            {
                if (_currentActivePokemonActionsView == null)
                {
                    CreateCurrentActivePokemonActionsView();
                }
                _currentActivePokemonActionsView.AddAbilityInteraction(ability, onAbilityAction);
            }
            else
            {
                if (_currentBenchedPokemonActionsView == null)
                {
                    CreateCurrentBenchedPokemonActionsView();
                }
                _currentBenchedPokemonActionsView.AddAbilityInteraction(ability, onAbilityAction);
            }
        }

        private void CreateCurrentBenchedPokemonActionsView()
        {
            _currentBenchedPokemonActionsView = Instantiate(_benchedPokemonActionsViewPrefab);
            _currentBenchedPokemonActionsView.Collider.Add(_col);
        }

        public void ShowBenchedPokemonActions()
        {
            Vector3 distance = GetBenchedPokemonActionsDistance();
            _currentBenchedPokemonActionsView.transform.position = transform.position + distance;
            _currentBenchedPokemonActionsView.transform.rotation = transform.rotation;
            _currentBenchedPokemonActionsView.Canvas.enabled = true;
        }

        private Vector3 GetBenchedPokemonActionsDistance()
        {
            var verticalDirection = transform.rotation * Vector3.up;
            var distance =
                (
                    _benchedPokemonActionsViewPrefab.GetComponent<RectTransform>().rect.height
                    + RectTransform.rect.height
                )
                / 2f
                * 1.1f
                * verticalDirection;
            return distance;
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
            Vector3 distance = GetActivePokemonActionsDistance();
            _currentActivePokemonActionsView.transform.position = transform.position + distance;
            _currentActivePokemonActionsView.transform.rotation = transform.rotation;
            _currentActivePokemonActionsView.Canvas.enabled = true;
        }

        private Vector3 GetActivePokemonActionsDistance()
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
