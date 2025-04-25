using System;
using gamecore.actionsystem;

namespace gamecore.action
{
    internal class DamageSystem : IActionPerformer<DealDamgeGA>
    {
        private static readonly Lazy<DamageSystem> lazy = new(() => new DamageSystem());
        public static DamageSystem INSTANCE => lazy.Value;

        private DamageSystem() { }

        private readonly ActionSystem _actionSystem = ActionSystem.INSTANCE;

        public void Enable()
        {
            _actionSystem.AttachPerformer<DealDamgeGA>(INSTANCE);
        }

        public void Disable()
        {
            _actionSystem.DetachPerformer<DealDamgeGA>();
        }

        public DealDamgeGA Perform(DealDamgeGA action)
        {
            action.Damage += action.ModifierBeforeWeaknessResistance;
            if (action.Target.IsActive())
                ApplyWeaknessResistance(action);
            action.Damage += action.ModifierAfterWeaknessResistance;
            action.Target.TakeDamage(action.Damage);
            return action;
        }

        private void ApplyWeaknessResistance(DealDamgeGA action)
        {
            if (action.Target.Weakness == action.Attacker.Type)
                action.Damage *= 2;
            else if (action.Target.Resistance == action.Attacker.Type)
                action.Damage -= 30;
        }
    }
}
