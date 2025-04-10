using System;
using System.Collections.Generic;
using gamecore.card;

namespace gamecore.game
{
    public class GameInteraction
    {
        public Action GameControllerMethod { get; }
        public Action<List<object>> GameControllerMethodWithTargets { get; }
        public ICard Card { get; }
        public GameInteractionType Type { get; }
        public List<object> PossibleTargets { get; } = null;
        public int NumberOfTargets { get; } = 0;

        public GameInteraction(
            Action<List<object>> gameControllerMethodWithTargets,
            GameInteractionType type,
            ICard card,
            List<object> possibleTargets,
            int numberOfTargets
        )
        {
            GameControllerMethodWithTargets = gameControllerMethodWithTargets;
            Type = type;
            Card = card;
            PossibleTargets = possibleTargets;
            NumberOfTargets = numberOfTargets;
        }

        public GameInteraction(Action gameControllerMethod, GameInteractionType type, ICard card)
        {
            GameControllerMethod = gameControllerMethod;
            Type = type;
            Card = card;
        }

        public GameInteraction(Action gameControllerMethod, GameInteractionType type)
            : this(gameControllerMethod, type, null) { }

        public GameInteraction() { }
    }

    public enum GameInteractionType
    {
        SelectActivePokemon,
        SetUpGame,
        PlayCard,
        EndTurn,
        ConfirmMulligans,
        SelectMulligans,
        Confirm,
    }
}
