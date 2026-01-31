using System.Collections.Generic;
using System.Linq;
using gamecore.card;
using gamecore.game.interaction;
using gamecore.serialization;

namespace gamecore.game.state
{
    class ShowSecondMulliganState : IGameState
    {
        public IGameState AdvanceSuccesfully()
        {
            return new SelectingMulliganCardsState();
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            var mulligans = gameController.Game.Mulligans;
            var mulliganNumberToSkip = mulligans.Values.Min(m => m.Count);

            var mulliganData = new MulliganData(new List<List<ICard>>(), null);

            foreach (var p in mulligans.Keys)
                if (mulligans[p].Count > mulliganNumberToSkip)
                    mulliganData = new MulliganData(
                        mulligans[p]
                            .GetRange(
                                mulliganNumberToSkip,
                                mulligans[p].Count - mulliganNumberToSkip
                            ),
                        p
                    );
            return new List<GameInteraction>()
            {
                new(
                    () => gameController.Confirm(),
                    GameInteractionType.ConfirmMulligans,
                    new() { mulliganData }
                ),
            };
        }

        public void OnAdvanced(Game game)
        {
            game.AwaitGeneralInteraction();
        }

        public ProtoBufTechnicalGameState ToProtoBuf()
        {
            return ProtoBufTechnicalGameState.GameStateShowSecondMulligan;
        }
    }
}
