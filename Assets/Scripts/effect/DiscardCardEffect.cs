using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using UnityEngine;

namespace gamecore.effect
{
    internal class DiscardCardEffect : IEffect
    {
        void IEffect.Perform(ICard card)
        {
            ActionSystem.INSTANCE.Perform(new DiscardCardsFromHandGA(new List<ICard> { card }));
        }
    }
}
