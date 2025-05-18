using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gamecore.game.state
{
    class SetupCompletedState : IGameState
    {
        public IGameState AdvanceSuccesfully()
        {
            return new ShowFirstMulliganState();
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            return new()
            {
                new(async () => await gameController.Confirm(), GameInteractionType.SetupCompleted),
            };
        }

        public Task OnAdvanced(Game game)
        {
            game.AwaitGeneralInteraction();
            return Task.CompletedTask;
        }
    }
}
