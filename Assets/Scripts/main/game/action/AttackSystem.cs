using System;
using gamecore.actionsystem;
using gamecore.game.action;

namespace gamecore.action
{
    internal class AttackSystem : IActionPerformer<AttackGA>, IActionPerformer<DrawPrizeCardsGA>
    {
        private static readonly Lazy<AttackSystem> lazy = new(() => new AttackSystem());
        public static AttackSystem INSTANCE => lazy.Value;

        private AttackSystem() { }

        private readonly ActionSystem _actionSystem = ActionSystem.INSTANCE;

        public void Enable()
        {
            _actionSystem.AttachPerformer<AttackGA>(INSTANCE);
            _actionSystem.AttachPerformer<DrawPrizeCardsGA>(INSTANCE);
        }

        public void Disable()
        {
            _actionSystem.DetachPerformer<AttackGA>();
            _actionSystem.DetachPerformer<DrawPrizeCardsGA>();
        }

        public AttackGA Perform(AttackGA action)
        {
            foreach (var effect in action.Attack.Effects)
            {
                effect.Perform(action.Attacker);
            }
            return action;
        }

        public DrawPrizeCardsGA Perform(DrawPrizeCardsGA action)
        {
            foreach (var playerEntry in action.NumberOfPrizeCardsPerPlayer)
            {
                var prizes = playerEntry.Key.Prizes.TakePrizes(playerEntry.Value);
                playerEntry.Key.Hand.AddCards(prizes);
            }
            return action;
        }
    }
}
