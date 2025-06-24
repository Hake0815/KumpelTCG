using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using gamecore.card;

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
