using System;
using System.Collections.Generic;
using System.Linq;
using gamecore.card;

namespace gamecore.game.interaction
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

        public GameInteractionJson ToSerializable()
        {
            var dataJson = new Dictionary<string, IGameInteractionDataJson>();

            foreach (var kvp in Data)
            {
                dataJson[kvp.Key] = kvp.Value.ToSerializable();
            }

            return new GameInteractionJson(Type, dataJson);
        }
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
        public IGameInteractionDataJson ToSerializable();
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

        public IGameInteractionDataJson ToSerializable()
        {
            return new MulliganDataJson(
                Mulligans
                    .Select(mulligan => mulligan.Select(card => card.ToSerializable()).ToList())
                    .ToList(),
                ((IPlayerLogic)Player).ToSerializable()
            );
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

        public IGameInteractionDataJson ToSerializable()
        {
            return new NumberDataJson(Number);
        }
    }

    public record TargetData : IGameInteractionData
    {
        public const string NAME = "Target";
        public string Name => NAME;

        public TargetData(
            int numberOfTargets,
            List<ICard> possibleTargets,
            ActionOnSelection targetAction,
            ActionOnSelection remainderAction
        )
        {
            NumberOfTargets = numberOfTargets;
            PossibleTargets = possibleTargets;
            TargetAction = targetAction;
            RemainderAction = remainderAction;
        }

        public List<ICard> PossibleTargets { get; }
        public int NumberOfTargets { get; } = 0;
        public ActionOnSelection TargetAction { get; }
        public ActionOnSelection RemainderAction { get; }

        public IGameInteractionDataJson ToSerializable()
        {
            return new TargetDataJson(
                NumberOfTargets,
                PossibleTargets.Select(card => card.ToSerializable()).ToList(),
                TargetAction,
                RemainderAction
            );
        }
    }

    public record ConditionalTargetData : IGameInteractionData
    {
        public const string NAME = "ConditionalTarget";
        public string Name => NAME;

        public ConditionalTargetData(
            IConditionalTargetQuery conditionalTargetQuery,
            List<ICard> possibleTargets,
            ActionOnSelection targetAction,
            ActionOnSelection remainderAction,
            bool isQuickSelection = true
        )
        {
            ConditionalTargetQuery = conditionalTargetQuery;
            PossibleTargets = possibleTargets;
            IsQuickSelection = isQuickSelection;
            TargetAction = targetAction;
            RemainderAction = remainderAction;
        }

        public List<ICard> PossibleTargets { get; }
        public IConditionalTargetQuery ConditionalTargetQuery { get; }
        public bool IsQuickSelection { get; }
        public ActionOnSelection TargetAction { get; }
        public ActionOnSelection RemainderAction { get; }

        public IGameInteractionDataJson ToSerializable()
        {
            return new ConditionalTargetDataJson(
                PossibleTargets.Select(card => card.ToSerializable()).ToList(),
                ConditionalTargetQuery.ToSerializable(),
                TargetAction,
                RemainderAction
            );
        }
    }

    public enum ActionOnSelection
    {
        Discard,
        TakeToHand,
        Evolve,
        AttachTo,
        Promote,
        Nothing,
        PutUnderDeck,
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

        public IGameInteractionDataJson ToSerializable()
        {
            return new InteractionCardDataJson(Card.ToSerializable());
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

        public IGameInteractionDataJson ToSerializable()
        {
            return new AttackDataJson(Attack.ToSerializable());
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

        public IGameInteractionDataJson ToSerializable()
        {
            return new WinnerDataJson(((IPlayerLogic)Winner).ToSerializable(), Message);
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

        public IGameInteractionDataJson ToSerializable()
        {
            return new SelectFromDataJson(
                SelectFrom,
                SelectionSource?.Select(card => card.ToSerializable()).ToList()
            );
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
