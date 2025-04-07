using System;
using gamecore.card;

namespace gamecore.game
{
    public class GameInteraction
    {
        public Action GameControllerMethod { get; set; }
        public ICard Card { get; set; }
        public GameInteractionType Type { get; set; }

        public GameInteraction(Action gameControllerMethod, GameInteractionType type, ICard card)
        {
            GameControllerMethod = gameControllerMethod;
            Type = type;
            Card = card;
        }

        public GameInteraction(Action gameControllerMethod, GameInteractionType type)
            : this(gameControllerMethod, type, null) { }
    }

    public enum GameInteractionType
    {
        SelectActivePokemon,
        SetUpGame,
        PlayCard,
        EndTurn,
    }
}
