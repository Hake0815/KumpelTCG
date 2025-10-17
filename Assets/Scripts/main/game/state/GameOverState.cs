using System.Collections.Generic;

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
            return new()
            {
                new GameInteraction(
                    () =>
                    {
                        gameController.ConfirmGameOver();
                    },
                    GameInteractionType.GameOver,
                    new() { new WinnerData(_winner, _message) }
                ),
            };
        }

        public void OnAdvanced(Game game) { }
    }
}
