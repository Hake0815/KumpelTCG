using System.Collections.Generic;
using System.Threading.Tasks;
using gamecore.game.interaction;
using gamecore.serialization;

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
                new(
                    () =>
                    {
                        gameController.Confirm();
                        return Task.CompletedTask;
                    },
                    GameInteractionType.SetupCompleted
                ),
            };
        }

        public void OnAdvanced(Game game)
        {
            game.AwaitGeneralInteraction();
        }

        public ProtoBufTechnicalGameState ToProtoBuf()
        {
            return ProtoBufTechnicalGameState.GameStateSetupCompleted;
        }
    }
}
