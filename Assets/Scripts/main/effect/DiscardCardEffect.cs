using System.Collections.Generic;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using UnityEngine;

namespace gamecore.effect
{
    internal class DiscardCardEffect : IEffect
    {
        async Task IEffect.Perform(ICardLogic card)
        {
            await ActionSystem.INSTANCE.Perform(
                new DiscardCardsFromHandGA(new List<ICardLogic> { card })
            );
        }
    }
}
