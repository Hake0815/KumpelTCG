using System.Collections.Generic;
using gamecore.game.interaction;

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
                new(() => gameController.Confirm(), GameInteractionType.SetupCompleted),
            };
        }

        public void OnAdvanced(Game game)
        {
            game.AwaitGeneralInteraction();
        }
    }
}
