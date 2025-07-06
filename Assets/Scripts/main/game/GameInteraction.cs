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
        public Dictionary<Type, IGameInteractionData> Data { get; } = new();

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

    public record NumberData : IGameInteractionData
    {
        public int Number { get; }

        public NumberData(int number)
        {
            Number = number;
        }
    }

    public record TargetData : IGameInteractionData
    {
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

        public InteractionCard(ICard card)
        {
            Card = card;
        }
    }

    public record AttackData : IGameInteractionData
    {
        public IAttack Attack { get; }

        public AttackData(IAttack card)
        {
            Attack = card;
        }
    }

    public record WinnerData : IGameInteractionData
    {
        public IPlayer Winner { get; }

        public WinnerData(IPlayer winner)
        {
            Winner = winner;
        }
    }

    public record SelectFromData : IGameInteractionData
    {
        public SelectFrom SelectFrom { get; }
        public List<ICard> SelectionSource { get; }

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
