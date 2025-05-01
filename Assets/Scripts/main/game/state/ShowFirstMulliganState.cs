using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamecore.card;

namespace gamecore.game.state
{
    internal class ShowFirstMulliganState : IGameState
    {
        private int _numberConfirmations = 0;

        public IGameState AdvanceSuccesfully()
        {
            _numberConfirmations++;
            if (_numberConfirmations == 2)
                return new SettingActivePokemonState();

            return this;
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            var mulligans = gameController.Game.Mulligans;
            var mulliganNumberToShow = mulligans.Values.Min(m => m.Count);

            var mulligansToShow = new Dictionary<IPlayer, List<List<ICard>>>();
            foreach (var p in mulligans.Keys)
                mulligansToShow.Add(p, mulligans[p].GetRange(0, mulliganNumberToShow));
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
            if (_numberConfirmations == 0)
                game.AwaitGeneralInteraction();
            return Task.CompletedTask;
        }
    }
}
