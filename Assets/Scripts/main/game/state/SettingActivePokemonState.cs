using System.Collections.Generic;
using gamecore.card;

namespace gamecore.game.state
{
    class SettingActivePokemonState : IGameState
    {
        private int _numberOfActivePokemonSelected = 0;

        public IGameState AdvanceSuccesfully()
        {
            _numberOfActivePokemonSelected++;
            if (_numberOfActivePokemonSelected == 2)
                return new SettingPrizeCardsState();

            return this;
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            if (player.ActivePokemon != null)
                return new();
            var interactions = new List<GameInteraction>();
            foreach (var basicPokemon in player.Hand.GetBasicPokemon())
            {
                interactions.Add(CreateGameInteraction(basicPokemon, gameController));
            }
            return interactions;
        }

        private static GameInteraction CreateGameInteraction(
            IPokemonCardLogic basicPokemon,
            GameController gameController
        )
        {
            return new GameInteraction(
                async () => await gameController.SelectActivePokemon(basicPokemon),
                GameInteractionType.SelectActivePokemon,
                new() { new InteractionCard(basicPokemon) }
            );
        }

        public void OnAdvanced(Game game)
        {
            game.AwaitInteraction();
        }
    }
}
