using System.Collections.Generic;
using gamecore.card;
using gamecore.game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace gameview
{
    public class MulliganView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text currentMulliganText;

        [SerializeField]
        private GameObject mulliganPanel;

        [SerializeField]
        private Transform cardContainer;

        [SerializeField]
        private Button previousButton;

        [SerializeField]
        private Button nextButton;

        private IPlayer player;
        private List<List<ICard>> mulligans;
        private int currentMulliganIndex = 0;

        public void SetUp(IPlayer player, List<List<ICard>> mulligans)
        {
            this.player = player;
            this.mulligans = mulligans;

            previousButton.onClick.AddListener(ShowPreviousMulligan);
            nextButton.onClick.AddListener(ShowNextMulligan);

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
                mulliganPanel.SetActive(false);
                return;
            }

            mulliganPanel.SetActive(true);
            currentMulliganText.text =
                $"Mulligan {currentMulliganIndex + 1}/{mulligans.Count} of {player.Name}";

            previousButton.interactable = currentMulliganIndex > 0;
            nextButton.interactable = currentMulliganIndex < mulligans.Count - 1;

            foreach (Transform child in cardContainer)
            {
                Destroy(child.gameObject);
            }

            var currentMulligan = mulligans[currentMulliganIndex];
            float cardSpacing = 170f; // Adjust this value to control space between cards
            float startX = -(currentMulligan.Count - 1) * cardSpacing / 2f; // Center the cards

            for (int i = 0; i < currentMulligan.Count; i++)
            {
                var card = currentMulligan[i];
                Debug.Log($"Creating card {card.Name}");
                var cardView = CardViewCreator.INSTANCE.CreateIn(card, cardContainer);
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
            if (previousButton != null)
                previousButton.onClick.RemoveListener(ShowPreviousMulligan);
            if (nextButton != null)
                nextButton.onClick.RemoveListener(ShowNextMulligan);
        }
    }
}
