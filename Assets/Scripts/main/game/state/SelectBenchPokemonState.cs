using System.Collections.Generic;
using gamecore.card;

namespace gamecore.game.state
{
    class SelectBenchPokemonState : IGameState
    {
        private bool _doneSelecting = false;

        public IGameState AdvanceSuccesfully()
        {
            if (_doneSelecting)
            {
                return new IdlePlayerTurnState();
            }
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
                    async () =>
                    {
                        _doneSelecting = true;
                        await gameController.StartFirstTurnOfGame();
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
                async () => await gameController.PlayCard(basicPokemon),
                GameInteractionType.PlayCard,
                new() { new InteractionCard(basicPokemon) }
            );
        }

        public void OnAdvanced(Game game)
        {
            game.AwaitInteraction();
        }
    }
}
