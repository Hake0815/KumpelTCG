using System;
using gamecore.actionsystem;

namespace gamecore.game.action
{
    public class DrawCardSystem : IActionPerformer<DrawCardGA>, IActionSubscriber<EndTurnGA>
    {
        private static readonly Lazy<DrawCardSystem> lazy = new(() => new DrawCardSystem());
        public static DrawCardSystem INSTANCE => lazy.Value;

        private DrawCardSystem() { }

        public void Enable()
        {
            ActionSystem.INSTANCE.AttachPerformer<DrawCardGA>(INSTANCE);
            ActionSystem.INSTANCE.SubscribeToGameAction<EndTurnGA>(INSTANCE, ReactionTiming.POST);
        }

        public void Disable()
        {
            ActionSystem.INSTANCE.DetachPerformer<DrawCardGA>();
            ActionSystem.INSTANCE.UnsubscribeFromGameAction<EndTurnGA>(INSTANCE, ReactionTiming.POST);
        }

        public EndTurnGA React(EndTurnGA endTurnGA)
        {
            var drawCardGA = new DrawCardGA(1, endTurnGA.NextPlayer);
            ActionSystem.INSTANCE.AddReaction(drawCardGA);
            return endTurnGA;
        }

        public DrawCardGA Perform(DrawCardGA drawCardGA)
        {
            if (drawCardGA.Amount != 1)
                throw new NotImplementedException("Can only draw one card for now!");
            drawCardGA.Player.Draw();
            return drawCardGA;
        }
    }
}
