using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using gamecore.card;
using UnityEngine;
using UnityEngine.UI;

namespace gameview
{
    public class TrainerCardView : CardView
    {
        public ITrainerCard TrainerCard
        {
            get => Card as ITrainerCard;
        }

        public Image Image { get; set; }

        public void Awake()
        {
            Image = GetComponentInChildren<Image>();
        }

        private void OnMouseUp()
        {
            col.enabled = false;
            var hitCollider = Physics2D.OverlapPoint(transform.position);
            col.enabled = true;
            if (hitCollider != null && hitCollider.TryGetComponent(out ICardDropArea cardDropArea))
            {
                if (cardDropArea.OnCardDropped(this))
                {
                    return;
                }
            }
            transform.position = positionBeforeDrag;
        }
    }
}
