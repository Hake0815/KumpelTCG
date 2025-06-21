using System.Collections.Generic;
using System.Threading.Tasks;

namespace gamecore.game.state
{
    class SettingPrizeCardsState : IGameState
    {
        public IGameState AdvanceSuccesfully()
        {
            return new ShowSecondMulliganState();
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            return new List<GameInteraction>()
            {
                new(
                    async () => await gameController.SetPrizeCards(),
                    GameInteractionType.SetPrizeCards
                ),
            };
        }

        public Task OnAdvanced(Game game)
        {
            game.AwaitGeneralInteraction();
            return Task.CompletedTask;
        }
    }
}
