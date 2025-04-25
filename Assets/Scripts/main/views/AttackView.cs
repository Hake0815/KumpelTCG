using System;
using System.Collections.Generic;
using gamecore.card;
using gamecore.game;
using UnityEngine;
using UnityEngine.UI;

namespace gameview
{
    public class AttackView : MonoBehaviour
    {
        [SerializeField]
        private Button _attackButtonPrefab;
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

        public void AddInteraction(IAttack attack, Action onAttackAction)
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
    }
}
