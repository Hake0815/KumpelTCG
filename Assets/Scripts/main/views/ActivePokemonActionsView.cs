using System;
using System.Collections.Generic;
using gamecore.card;
using gamecore.game;
using UnityEngine;
using UnityEngine.UI;

namespace gameview
{
    public class ActivePokemonActionsView : MonoBehaviour
    {
        [SerializeField]
        private Button _abilityButtonPrefab;

        [SerializeField]
        private Button _attackButtonPrefab;

        [SerializeField]
        private Button _retreatButton;

        [SerializeField]
        private RectTransform _energyCost;

        [SerializeField]
        private Canvas _typeIconPrefab;
        private float _buttonSpacing;
        private Vector3 _verticalDirection;
        public Button AttackButtonPrefab => _attackButtonPrefab;
        public Canvas Canvas { get; set; }
        public HashSet<Collider2D> Collider { get; } = new();

        public List<IAttack> Attacks { get; } = new();

        private void Awake()
        {
            _buttonSpacing = _attackButtonPrefab.GetComponent<RectTransform>().rect.height * 1.1f;
            _verticalDirection = transform.rotation * Vector3.up;
            Canvas = GetComponent<Canvas>();
            InputHandler.INSTANCE.OnMouseLeftClick += HideThis;
            InputHandler.INSTANCE.OnEsc += HideThis;
        }

        private void OnDestroy()
        {
            if (InputHandler.INSTANCE == null)
                return;
            InputHandler.INSTANCE.OnMouseLeftClick -= HideThis;
            InputHandler.INSTANCE.OnEsc -= HideThis;
        }

        private void HideThis(Collider2D d)
        {
            if (!Collider.Contains(d))
                HideThis();
        }

        private void HideThis()
        {
            Canvas.enabled = false;
        }

        public void AddAbilityInteraction(IAbility ability, Action onAbilityAction)
        {
            var abilityButton = Instantiate(
                _abilityButtonPrefab,
                transform.position + _buttonSpacing * _verticalDirection,
                transform.rotation
            );
            abilityButton.transform.SetParent(transform);
            abilityButton.GetComponent<AbilityButtonView>().Show(ability);
            Collider.Add(abilityButton.GetComponent<Collider2D>());
            abilityButton.onClick.AddListener(() => onAbilityAction.Invoke());
        }

        public void AddAttackInteraction(IAttack attack, Action onAttackAction)
        {
            Attacks.Add(attack);
            int i = Attacks.Count - 1;
            var attackButton = Instantiate(
                _attackButtonPrefab,
                transform.position - i * _buttonSpacing * _verticalDirection,
                transform.rotation
            );
            attackButton.transform.SetParent(transform);
            attackButton.GetComponent<AttackButtonView>().Show(attack);
            Collider.Add(attackButton.GetComponent<Collider2D>());
            attackButton.onClick.AddListener(() => onAttackAction.Invoke());
        }

        public void DestroyThis()
        {
            Destroy(gameObject);
        }

        public void AddRetreatInteraction(int cost, Action onRetreatAction)
        {
            _retreatButton.gameObject.SetActive(true);
            _retreatButton.onClick.AddListener(() => onRetreatAction.Invoke());
            Collider.Add(_retreatButton.GetComponent<Collider2D>());
            PlaceEnergyCostIcons(cost);
        }

        private void PlaceEnergyCostIcons(int cost)
        {
            var transform = _energyCost.transform;
            var spacing = _typeIconPrefab.GetComponent<RectTransform>().rect.width * 1.05f;
            var width = cost * spacing;
            var horizontalDirection = transform.rotation * Vector3.right;
            var firstPosition = transform.position - (width - spacing) / 2f * horizontalDirection;
            for (int i = 0; i < cost; i++)
            {
                var typeIcon = Instantiate(
                    _typeIconPrefab,
                    firstPosition + i * spacing * horizontalDirection,
                    transform.rotation
                );
                var iconImage = typeIcon.GetComponentInChildren<Image>();
                iconImage.sprite = SpriteRegistry.INSTANCE.GetTypeIcon(PokemonType.Colorless);
                typeIcon.transform.SetParent(transform);
            }
        }
    }
}
