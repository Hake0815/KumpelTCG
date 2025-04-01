using System.Collections.Generic;
using gamecore.card;
using gamecore.game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace gameview
{
    public class MulliganView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _currentMulliganText;

        [SerializeField]
        private GameObject _mulliganPanel;

        [SerializeField]
        private Transform _cardContainer;

        [SerializeField]
        private Button _previousButton;

        [SerializeField]
        private Button _nextButton;

        [SerializeField]
        private Button _doneButton;

        private IPlayer player;
        private List<List<ICard>> mulligans;
        private int currentMulliganIndex = 0;

        public void SetUp(IPlayer player, List<List<ICard>> mulligans)
        {
            this.player = player;
            this.mulligans = mulligans;

            _previousButton.onClick.AddListener(ShowPreviousMulligan);
            _nextButton.onClick.AddListener(ShowNextMulligan);
            _doneButton.onClick.AddListener(Done);
            UpdateView();
        }

        private void ShowPreviousMulligan()
        {
            if (currentMulliganIndex > 0)
            {
                currentMulliganIndex--;
                UpdateView();
            }
        }

        private void ShowNextMulligan()
        {
            if (currentMulliganIndex < mulligans.Count - 1)
            {
                currentMulliganIndex++;
                UpdateView();
            }
        }

        private void UpdateView()
        {
            if (mulligans == null || mulligans.Count == 0)
            {
                _mulliganPanel.SetActive(false);
                return;
            }

            _mulliganPanel.SetActive(true);
            _currentMulliganText.text =
                $"Mulligan {currentMulliganIndex + 1}/{mulligans.Count} of {player.Name}";

            _previousButton.interactable = currentMulliganIndex > 0;
            _nextButton.interactable = currentMulliganIndex < mulligans.Count - 1;

            foreach (Transform child in _cardContainer)
            {
                Destroy(child.gameObject);
            }

            var currentMulligan = mulligans[currentMulliganIndex];
            float cardSpacing = 170f; // Adjust this value to control space between cards
            float startX = -(currentMulligan.Count - 1) * cardSpacing / 2f; // Center the cards

            for (int i = 0; i < currentMulligan.Count; i++)
            {
                var card = currentMulligan[i];
                var cardView = CardViewCreator.INSTANCE.CreateIn(card, _cardContainer);
                cardView.transform.localScale = new Vector3(150f, 150f, 150f);

                var rectTransform = cardView.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = new Vector2(startX + (i * cardSpacing), 0);
                    rectTransform.localRotation = Quaternion.identity; // Ensure cards are not rotated
                    rectTransform.SetAsLastSibling(); // Ensure proper layering
                }
            }
        }

        private void OnDestroy()
        {
            _previousButton.onClick.RemoveListener(ShowPreviousMulligan);
            _nextButton.onClick.RemoveListener(ShowNextMulligan);
            _doneButton.onClick.RemoveListener(Done);
        }

        public void AddDoneListener(UnityAction action)
        {
            _doneButton.onClick.AddListener(action);
        }

        private void Done()
        {
            Destroy(gameObject);
        }
    }
}
