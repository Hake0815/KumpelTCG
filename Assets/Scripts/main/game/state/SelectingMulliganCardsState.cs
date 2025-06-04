using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.game;
using gamecore.game.action;

namespace gamecore.game.state
{
    class SelectingMulliganCardsState : IGameState
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
            int mulliganDifference = Math.Max(
                mulligans[player.Opponent].Count - mulligans[player].Count,
                0
            );

            if (mulliganDifference <= 0)
                return new();

            var interactions = new List<GameInteraction>();
            for (int i = 0; i < mulliganDifference + 1; i++)
            {
                var mulliganCount = i;
                interactions.Add(
                    new(
                        gameControllerMethod: async () =>
                            await gameController.SelectMulligans(mulliganCount, player),
                        type: GameInteractionType.SelectMulligans,
                        data: new() { new NumberData(i) }
                    )
                );
            }
            return interactions;
        }

        public async Task OnAdvanced(Game game)
        {
            // game.SetPrizeCards();
            await ActionSystem.INSTANCE.Perform(new SetPrizeCardsGA());
            var mulligans = game.Mulligans.Values.ToList();
            if (mulligans[0].Count != mulligans[1].Count)
                game.AwaitInteraction();
            else
                await game.AdvanceGameState();
        }
    }
}
