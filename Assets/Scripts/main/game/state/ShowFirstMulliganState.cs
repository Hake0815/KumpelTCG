using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using gamecore.card;
using gamecore.game.interaction;

namespace gamecore.game.state
{
    class ShowFirstMulliganState : IGameState
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

            var playerToShow = mulligans.Keys.ToList()[_numberConfirmations];
            var mulligansToShow = mulligans[playerToShow].GetRange(0, mulliganNumberToShow);
            return new List<GameInteraction>()
            {
                new(
                    () => gameController.Confirm(),
                    GameInteractionType.ConfirmMulligans,
                    new() { new MulliganData(mulligansToShow, playerToShow) }
                ),
            };
        }

        public void OnAdvanced(Game game)
        {
            game.AwaitGeneralInteraction();
        }
    }
}
