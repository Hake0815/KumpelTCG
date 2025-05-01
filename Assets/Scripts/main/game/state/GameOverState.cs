using System.Collections.Generic;
using System.Threading.Tasks;

namespace gamecore.game.state
{
    internal class GameOverState : IGameState
    {
        private readonly IPlayerLogic _winner;

        public GameOverState(IPlayerLogic winner)
        {
            _winner = winner;
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
                    _ => { },
                    GameInteractionType.GameOver,
                    new() { new WinnerData(_winner) }
                ),
            };
        }

        public Task OnAdvanced(Game game)
        {
            return Task.CompletedTask;
        }
    }
}
