using System.Collections.Generic;
using System.Threading.Tasks;
using gamecore.game.interaction;
using gamecore.serialization;

namespace gamecore.game.state
{
    class GameOverState : IGameState
    {
        private readonly IPlayerLogic _winner;
        private readonly string _message;

        public GameOverState(IPlayerLogic winner, string message)
        {
            _winner = winner;
            _message = message;
        }

        public IGameState AdvanceSuccesfully()
        {
            return this;
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            return new List<GameInteraction>
            {
                new(
                    () => Task.CompletedTask,
                    GameInteractionType.GameOver,
                    new List<IGameInteractionData> { new WinnerData(_winner, _message) }
                ),
            };
        }

        public void OnAdvanced(Game game)
        {
            game.FinishGameLog();
            game.AwaitGeneralInteraction();
        }

        public ProtoBufTechnicalGameState ToProtoBuf()
        {
            return ProtoBufTechnicalGameState.GameStateGameOver;
        }
    }
}
