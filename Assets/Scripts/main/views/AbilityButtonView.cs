using gamecore.card;
using TMPro;
using UnityEngine;

namespace gameview
{
    public class AbilityButtonView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _abilityName;

        public void Show(IAbility ability)
        {
            _abilityName.text = ability.Name;
        }
    }
}
