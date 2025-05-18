using System;
using System.Collections.Generic;
using gamecore.card;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace gameview
{
    public class BenchedPokemonActionsView : MonoBehaviour
    {
        [SerializeField]
        private Button _abilityButton;
        public Canvas Canvas { get; set; }
        public HashSet<Collider2D> Collider { get; } = new();

        private void Awake()
        {
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
            _abilityButton.gameObject.SetActive(true);
            _abilityButton.GetComponentInChildren<TMP_Text>().text = ability.Name;
            Collider.Add(_abilityButton.GetComponent<Collider2D>());
            _abilityButton.onClick.AddListener(() => onAbilityAction.Invoke());
        }

        public void DestroyThis()
        {
            Destroy(gameObject);
        }
    }
}
