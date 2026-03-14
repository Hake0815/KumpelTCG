using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class PerformAbilityGA : GameAction
    {
        public IPokemonCardLogic Pokemon;

        [JsonConstructor]
        public PerformAbilityGA(IPokemonCardLogic pokemon)
        {
            Pokemon = pokemon;
        }
    }
}
