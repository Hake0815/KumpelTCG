using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using UnityEngine;

namespace gamecore.effect
{
    internal class DiscardCardEffect : IEffect
    {
        void IEffect.Perform(ICardLogic card)
        {
            ActionSystem.INSTANCE.Perform(
                new DiscardCardsFromHandGA(new List<ICardLogic> { card })
            );
        }
    }
}
