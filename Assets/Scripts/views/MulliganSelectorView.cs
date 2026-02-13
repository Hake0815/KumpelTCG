using System;
using System.Collections.Generic;
using gamecore.card;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace gameview
{
    public class MulliganSelectorView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Dropdown _dropdown;

        [SerializeField]
        private Button _confirmButton;
        private readonly List<string> _options = new();
        private readonly Dictionary<string, Action> _optionAction = new();

        private void Awake()
        {
            _dropdown.ClearOptions();
            _confirmButton.onClick.RemoveAllListeners();
            _confirmButton.onClick.AddListener(() =>
            {
                var selectedValue = _options[_dropdown.value];
                _optionAction.GetValueOrDefault(selectedValue).Invoke();
                Destroy(gameObject);
            });
        }

        internal void AddOption(string mulliganOption, Action onConfirm)
        {
            _options.Add(mulliganOption);
            _optionAction.Add(mulliganOption, onConfirm);

            _dropdown.AddOptions(new List<string>() { mulliganOption });
            _dropdown.value = _options.Count - 1;
            _dropdown.RefreshShownValue();
        }
    }
}
