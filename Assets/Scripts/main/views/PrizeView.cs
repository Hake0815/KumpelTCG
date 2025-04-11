using System;
using DG.Tweening;
using gamecore.card;
using gamecore.game;
using TMPro;
using UnityEngine;

namespace gameview
{
    public class PrizeView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _countText;
        private IPrizes _prizes;
        private RectTransform _rectTransform;

        public void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void SetUp(IPlayer player)
        {
            _prizes = player.Prizes;
            _countText.text = _prizes.CardCount.ToString();
            OnEnable();
        }

        private void OnEnable()
        {
            if (_prizes != null)
            {
                _prizes.CardCountChanged += UpdateView;
            }
        }

        private void OnDisable()
        {
            if (_prizes != null)
            {
                _prizes.CardCountChanged -= UpdateView;
            }
        }

        private void UpdateView()
        {
            _countText.text = _prizes.CardCount.ToString();
            var horizontalSpacing = _rectTransform.rect.width / 3f;
            var verticalSpacing = _rectTransform.rect.height / 2f;
            var relativeRight = _rectTransform.rotation * Vector3.right;
            var relativeDown = _rectTransform.rotation * Vector3.down;
            var firstPosition =
                _rectTransform.position
                + horizontalSpacing / 2f * relativeRight
                + verticalSpacing / 2f * relativeDown;
            int i = 0;
            foreach (var prizeCard in _prizes)
            {
                var cardView = CardViewRegistry.INSTANCE.Get(prizeCard);
                if (i < 3)
                    cardView.transform.DOMove(
                        firstPosition + i * horizontalSpacing * relativeRight,
                        0.25f
                    );
                else
                    cardView.transform.DOMove(
                        firstPosition
                            + (i - 3) * horizontalSpacing * relativeRight
                            + verticalSpacing * relativeDown,
                        0.25f
                    );
                cardView.transform.DOLocalRotateQuaternion(_rectTransform.rotation, 0.25f);
                i++;
            }
        }
    }
}
