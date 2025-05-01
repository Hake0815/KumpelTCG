using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamecore.game;

namespace gamecore.game.state
{
    internal class SelectingMulliganCardsState : IGameState
    {
        public IGameState AdvanceSuccesfully()
        {
            return new SelectBenchPokemonState();
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            var mulligans = gameController.Game.Mulligans;
            var numberOfMulligansOfPlayer = mulligans[player].Count;
            int mulliganDifference = 0;
            foreach (var mulligan in mulligans.Values)
            {
                mulliganDifference = Math.Max(
                    mulligan.Count - numberOfMulligansOfPlayer,
                    mulliganDifference
                );
            }

            if (mulliganDifference <= 0)
                return new();

            var targetData = new TargetData(
                numberOfTargets: 1,
                possibleTargets: Enumerable.Range(0, mulliganDifference + 1).Cast<object>().ToList()
            );
            return new List<GameInteraction>()
            {
                new(
                    gameControllerMethodWithTargets: (targets) =>
                        gameController.SelectMulligans((int)targets[0], player),
                    type: GameInteractionType.SelectMulligans,
                    data: new() { targetData }
                ),
            };
        }

        public async Task OnAdvanced(Game game)
        {
            game.SetPrizeCards();
            var mulligans = game.Mulligans.Values.ToList();
            if (mulligans[0].Count != mulligans[1].Count)
                game.AwaitInteraction();
            else
                await game.AdvanceGameState();
        }
    }
}
