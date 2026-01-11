using System.Collections.Generic;
using System.Threading.Tasks;
using gamecore.game.interaction;
using gamecore.serialization;

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

        public void OnAdvanced(Game game)
        {
            game.AwaitGeneralInteraction();
        }

        public ProtoBufTechnicalGameState ToProtoBuf()
        {
            return ProtoBufTechnicalGameState.GameStateSettingPrizeCards;
        }
    }
}
