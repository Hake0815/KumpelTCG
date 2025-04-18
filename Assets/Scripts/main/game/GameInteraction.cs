using System;
using System.Collections.Generic;
using gamecore.card;

namespace gamecore.game
{
    public record GameInteraction
    {
        public Action GameControllerMethod { get; }
        public Action<List<object>> GameControllerMethodWithTargets { get; }
        public GameInteractionType Type { get; }
        public Dictionary<Type, IGameInteractionData> Data { get; } = new();

        public GameInteraction(
            Action<List<object>> gameControllerMethodWithTargets,
            GameInteractionType type,
            List<IGameInteractionData> data
        )
        {
            GameControllerMethodWithTargets = gameControllerMethodWithTargets;
            Type = type;
            foreach (var datum in data)
            {
                Data.Add(datum.GetType(), datum);
            }
        }

        public GameInteraction(
            Action gameControllerMethod,
            GameInteractionType type,
            List<IGameInteractionData> data
        )
        {
            GameControllerMethod = gameControllerMethod;
            Type = type;
            foreach (var datum in data)
            {
                Data.Add(datum.GetType(), datum);
            }
        }

        public GameInteraction(Action gameControllerMethod, GameInteractionType type)
            : this(gameControllerMethod, type, new()) { }

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
        SetupCompleted,
        PlayCardWithTargets,
    }

    public interface IGameInteractionData { }

    public record MulliganData : IGameInteractionData
    {
        public Dictionary<IPlayer, List<List<ICard>>> Mulligans { get; }

        public MulliganData(Dictionary<IPlayer, List<List<ICard>>> mulligans)
        {
            Mulligans = mulligans;
        }
    }

    public record TargetData : IGameInteractionData
    {
        public TargetData(int numberOfTargets, List<object> possibleTargets)
        {
            NumberOfTargets = numberOfTargets;
            PossibleTargets = possibleTargets;
        }

        public List<object> PossibleTargets { get; }
        public int NumberOfTargets { get; } = 0;
    }

    public record InteractionCard : IGameInteractionData
    {
        public ICard Card { get; }

        public InteractionCard(ICard card)
        {
            Card = card;
        }
    }
}
