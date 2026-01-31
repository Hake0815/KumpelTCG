using gamecore.actionsystem;

namespace gamecore.game.action
{
    class ClearPokemonTurnTraitsGA : GameAction
    {
        public ClearPokemonTurnTraitsGA(IPlayerLogic player)
        {
            Player = player;
        }

        public IPlayerLogic Player { get; }
    }
}
