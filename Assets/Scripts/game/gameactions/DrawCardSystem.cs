using System;
using gamecore.actionsystem;

namespace gamecore.game.action
{
    public class DrawCardSystem
    {
        private static readonly Lazy<DrawCardSystem> lazy = new(() => new DrawCardSystem());
        public static DrawCardSystem INSTANCE => lazy.Value;

        private DrawCardSystem() { }

        public void Enable()
        {
            ActionSystem.INSTANCE.AttachPerformer<DrawCardGA>(DrawCardPerformer);
            ActionSystem.INSTANCE.SubscribeReaction<EndTurnGA>(
                DrawForTurnReaction,
                ReactionTiming.POST
            );
        }

        public void Disable()
        {
            ActionSystem.INSTANCE.DetachPerformer<DrawCardGA>();
            ActionSystem.INSTANCE.UnsubscribeReaction<EndTurnGA>(
                DrawForTurnReaction,
                ReactionTiming.POST
            );
        }

        private EndTurnGA DrawForTurnReaction(EndTurnGA endTurnGA)
        {
            var drawCardGA = new DrawCardGA(1, endTurnGA.NextPlayer);
            ActionSystem.INSTANCE.AddReaction(drawCardGA);
            return endTurnGA;
        }

        private DrawCardGA DrawCardPerformer(DrawCardGA drawCardGA)
        {
            if (drawCardGA.Amount != 1)
                throw new NotImplementedException("Can only draw one card for now!");
            drawCardGA.Player.Draw();
            return drawCardGA;
        }
    }
}
