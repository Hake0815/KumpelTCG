using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gamecore.game.state
{
    internal class CreatedState : IGameState
    {
        public IGameState AdvanceSuccesfully()
        {
            return new SetupCompletedState();
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            var gameInteraction = new GameInteraction(
                async () => await gameController.SetUpGame(),
                GameInteractionType.SetUpGame
            );
            return new List<GameInteraction>() { gameInteraction };
        }

        public Task OnAdvanced(Game game)
        {
            return Task.CompletedTask;
        }
    }
}
