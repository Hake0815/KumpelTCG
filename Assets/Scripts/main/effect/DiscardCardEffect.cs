using System.Collections.Generic;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using UnityEngine;

namespace gamecore.effect
{
    class DiscardCardEffect : IEffect
    {
        public void Perform(ICardLogic card)
        {
            ActionSystem.INSTANCE.AddReaction(new DiscardCardsGA(new List<ICardLogic> { card }));
        }
    }
}
