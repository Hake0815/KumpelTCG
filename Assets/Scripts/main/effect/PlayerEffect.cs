using gamecore.actionsystem;
using gamecore.game;

namespace gamecore.effect
{
    public abstract class PlayerEffectAbstract
    {
        private protected ActionSystem _actionSystem;

        private protected PlayerEffectAbstract(ActionSystem actionSystem)
        {
            _actionSystem = actionSystem;
        }

        public abstract PlayerEffectJson ToSerializable();
    }
}
