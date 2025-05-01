using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamecore.card;

namespace gamecore.game.state
{
    internal class ShowSecondMulliganState : IGameState
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

            var mulligansToShow = new Dictionary<IPlayer, List<List<ICard>>>();
            foreach (var p in mulligans.Keys)
                if (mulligans[p].Count > mulliganNumberToSkip)
                    mulligansToShow.Add(
                        p,
                        mulligans[p]
                            .GetRange(
                                mulliganNumberToSkip,
                                mulligans[p].Count - mulliganNumberToSkip
                            )
                    );
            return new List<GameInteraction>()
            {
                new(
                    gameController.Confirm,
                    GameInteractionType.ConfirmMulligans,
                    new() { new MulliganData(mulligansToShow) }
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
