using System;
using System.Collections.Generic;
using gamecore.card;

namespace gamecore.game
{
    public record GameInteraction
    {
        public Action GameControllerMethod { get; }
        public Action<List<ICard>> GameControllerMethodWithTargets { get; }
        public GameInteractionType Type { get; }
        public Dictionary<string, IGameInteractionData> Data { get; } = new();

        public GameInteraction(
            Action<List<ICard>> gameControllerMethodWithTargets,
            GameInteractionType type,
            List<IGameInteractionData> data
        )
        {
            GameControllerMethodWithTargets = gameControllerMethodWithTargets;
            Type = type;
            foreach (var datum in data)
            {
                Data.Add(datum.Name, datum);
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
                Data.Add(datum.Name, datum);
            }
        }

        public GameInteraction(Action gameControllerMethod, GameInteractionType type)
            : this(gameControllerMethod, type, new()) { }
    }

    public enum GameInteractionType
    {
        SelectActivePokemon,
        PlayCard,
        EndTurn,
        ConfirmMulligans,
        SelectMulligans,
        Confirm,
        SetupCompleted,
        PlayCardWithTargets,
        PerformAttack,
        GameOver,
        SelectCards,
        Retreat,
        PerformAbility,
        SetPrizeCards,
        ReplayNextAction,
    }

    public interface IGameInteractionData
    {
        public String Name { get; }
    }

    public record MulliganData : IGameInteractionData
    {
        public List<List<ICard>> Mulligans { get; }
        public IPlayer Player { get; }
        public const string NAME = "Mulligan";
        public string Name => NAME;

        public MulliganData(List<List<ICard>> mulligans, IPlayer player)
        {
            Mulligans = mulligans;
            Player = player;
        }
    }

    public record NumberData : IGameInteractionData
    {
        public int Number { get; }

        public const string NAME = "Number";
        public string Name => NAME;

        public NumberData(int number)
        {
            Number = number;
        }
    }

    public record TargetData : IGameInteractionData
    {
        public const string NAME = "Target";
        public string Name => NAME;

        public TargetData(int numberOfTargets, List<ICard> possibleTargets)
        {
            NumberOfTargets = numberOfTargets;
            PossibleTargets = possibleTargets;
        }

        public List<ICard> PossibleTargets { get; }
        public int NumberOfTargets { get; } = 0;
    }

    public record ConditionalTargetData : IGameInteractionData
    {
        public const string NAME = "ConditionalTarget";
        public string Name => NAME;

        public ConditionalTargetData(
            Predicate<List<ICard>> conditionOnSelection,
            List<ICard> possibleTargets,
            bool isQuickSelection = true
        )
        {
            ConditionOnSelection = conditionOnSelection;
            PossibleTargets = possibleTargets;
            IsQuickSelection = isQuickSelection;
        }

        public List<ICard> PossibleTargets { get; }
        public Predicate<List<ICard>> ConditionOnSelection { get; }
        public bool IsQuickSelection { get; }
    }

    public record InteractionCard : IGameInteractionData
    {
        public ICard Card { get; }
        public const string NAME = "InteractionCard";
        public string Name => NAME;

        public InteractionCard(ICard card)
        {
            Card = card;
        }
    }

    public record AttackData : IGameInteractionData
    {
        public IAttack Attack { get; }
        public const string NAME = "Attack";
        public string Name => NAME;

        public AttackData(IAttack card)
        {
            Attack = card;
        }
    }

    public record WinnerData : IGameInteractionData
    {
        public IPlayer Winner { get; }
        public string Message { get; }
        public const string NAME = "Winner";
        public string Name => NAME;

        public WinnerData(IPlayer winner, string message)
        {
            Winner = winner;
            Message = message;
        }
    }

    public record SelectFromData : IGameInteractionData
    {
        public SelectFrom SelectFrom { get; }
        public List<ICard> SelectionSource { get; }
        public const string NAME = "SelectFrom";
        public string Name => NAME;

        public SelectFromData(SelectFrom selectFrom, List<ICard> selectionsource)
        {
            SelectFrom = selectFrom;
            SelectionSource = selectionsource;
        }

        public SelectFromData(SelectFrom selectFrom)
        {
            SelectFrom = selectFrom;
        }
    }

    public enum SelectFrom
    {
        InPlay,
        Floating,
        Deck,
        DiscardPile,
    }
}
