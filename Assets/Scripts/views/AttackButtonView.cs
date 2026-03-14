using gamecore.card;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace gameview
{
    public class AttackButtonView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _attackNameText;

        [SerializeField]
        private TMP_Text _attackDamageText;

        [SerializeField]
        private GameObject _energyCost;

        [SerializeField]
        private Canvas _typeIconPrefab;

        private IAttack _attack;

        public void Show(IAttack attack)
        {
            _attack = attack;
            _attackNameText.text = attack.Name;
            _attackDamageText.text = attack.Damage.ToString();
            ShowEnergyCost();
        }

        private void ShowEnergyCost()
        {
            var transform = _energyCost.transform;
            var rectTransform = _energyCost.GetComponent<RectTransform>();
            var width = rectTransform.rect.width;
            var spacing = _typeIconPrefab.GetComponent<RectTransform>().rect.width * 1.05f;
            var horizontalDirection = transform.rotation * Vector3.right;
            var firstPosition = transform.position - (width - spacing) / 2f * horizontalDirection;
            int i = 0;
            foreach (var energy in _attack.Cost)
            {
                var typeIcon = Instantiate(
                    _typeIconPrefab,
                    firstPosition + i * spacing * horizontalDirection,
                    transform.rotation
                );
                var iconImage = typeIcon.GetComponentInChildren<Image>();
                iconImage.sprite = SpriteRegistry.INSTANCE.GetTypeIcon(energy);
                typeIcon.transform.SetParent(transform);
                i++;
            }
        }
    }
}
