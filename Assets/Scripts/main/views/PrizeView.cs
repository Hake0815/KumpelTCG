using System;
using System.Collections.Generic;
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
                _prizes.PrizesTaken += RevealPrizeCard;
            }
        }

        private static void RevealPrizeCard(List<ICard> list)
        {
            foreach (var card in list)
            {
                var cardView = CardViewRegistry.INSTANCE.Get(card);
                cardView.FaceUp = true;
            }
        }

        private void OnDisable()
        {
            if (_prizes != null)
            {
                _prizes.CardCountChanged -= UpdateView;
                _prizes.PrizesTaken -= RevealPrizeCard;
            }
        }

        private void UpdateView(List<ICard> cards)
        {
            UIQueue.INSTANCE.Queue(
                (OnUICompleted) =>
                {
                    _countText.text = cards.Count.ToString();
                    var horizontalSpacing = _rectTransform.rect.width / 3f;
                    var verticalSpacing = _rectTransform.rect.height / 2f;
                    var relativeRight = _rectTransform.rotation * Vector3.right;
                    var relativeDown = _rectTransform.rotation * Vector3.down;
                    var firstPosition =
                        _rectTransform.position
                        + horizontalSpacing / 2f * relativeRight
                        + verticalSpacing / 2f * relativeDown;
                    int i = 0;
                    foreach (var prizeCard in cards)
                    {
                        var cardView = CardViewRegistry.INSTANCE.Get(prizeCard);
                        if (i < 3)
                            cardView.transform.DOMove(
                                firstPosition + i * horizontalSpacing * relativeRight,
                                AnimationSpeedHolder.AnimationSpeed
                            );
                        else
                            cardView.transform.DOMove(
                                firstPosition
                                    + (i - 3) * horizontalSpacing * relativeRight
                                    + verticalSpacing * relativeDown,
                                AnimationSpeedHolder.AnimationSpeed
                            );
                        cardView.transform.DOLocalRotateQuaternion(
                            _rectTransform.rotation,
                            AnimationSpeedHolder.AnimationSpeed
                        );
                        i++;
                    }
                    OnUICompleted.Invoke();
                }
            );
        }

        public void UpdateView()
        {
            UpdateView(_prizes.Cards);
        }
    }
}
