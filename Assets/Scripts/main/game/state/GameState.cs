using System.Collections.Generic;
using gamecore.game.interaction;

namespace gamecore.game.state
{
    internal interface IGameState
    {
        IGameState AdvanceSuccesfully();
        List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        );
        void OnAdvanced(Game game);
    }
}
