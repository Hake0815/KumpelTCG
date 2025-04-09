using System;
using System.Collections.Generic;
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

        public void SetUp(List<object> possibleValues, Action<object> OnConfirm)
        {
            _dropdown.ClearOptions();
            List<string> options = new();
            foreach (var value in possibleValues)
            {
                options.Add(value.ToString());
            }
            _dropdown.AddOptions(options);
            _dropdown.value = possibleValues.Count - 1;
            _dropdown.RefreshShownValue();

            _confirmButton.onClick.RemoveAllListeners();
            _confirmButton.onClick.AddListener(() =>
            {
                object selectedValue = possibleValues[_dropdown.value];
                OnConfirm(selectedValue);
                Destroy(gameObject);
            });
        }
    }
}
