using System.Collections.Generic;
using System.Threading.Tasks;

namespace gamecore.game.state
{
    internal class StartingGameState : IGameState
    {
        public IGameState AdvanceSuccesfully()
        {
            return new IdlePlayerTurnState();
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            return new();
        }

        public async Task OnAdvanced(Game game)
        {
            await game.StartGame();
            await game.AdvanceGameState();
        }
    }
}
