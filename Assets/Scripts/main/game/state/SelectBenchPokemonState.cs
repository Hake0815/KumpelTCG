using System.Collections.Generic;
using System.Threading.Tasks;
using gamecore.card;

namespace gamecore.game.state
{
    internal class SelectBenchPokemonState : IGameState
    {
        private bool _doneSelecting = false;

        public IGameState AdvanceSuccesfully()
        {
            if (_doneSelecting)
                return new StartingGameState();
            else
                return this;
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            var interactions = new List<GameInteraction>();
            foreach (var basicPokemon in player.Hand.GetBasicPokemon())
            {
                interactions.Add(CreateGameInteraction(basicPokemon, gameController));
            }
            interactions.Add(
                new(
                    () =>
                    {
                        _doneSelecting = true;
                        gameController.Confirm();
                    },
                    GameInteractionType.Confirm
                )
            );
            return interactions;
        }

        private static GameInteraction CreateGameInteraction(
            ICardLogic basicPokemon,
            GameController gameController
        )
        {
            return new GameInteraction(
                () => gameController.PlayCard(basicPokemon),
                GameInteractionType.PlayCard,
                new() { new InteractionCard(basicPokemon) }
            );
        }

        public Task OnAdvanced(Game game)
        {
            game.AwaitInteraction();
            return Task.CompletedTask;
        }
    }
}
