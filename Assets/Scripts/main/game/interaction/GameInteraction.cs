using System;
using System.Collections.Generic;
using System.Linq;
using gamecore.card;
using gamecore.serialization;

namespace gamecore.game.interaction
{
    public record GameInteraction
    {
        public Action GameControllerMethod { get; }
        public Action<List<ICard>> GameControllerMethodWithTargets { get; }
        public GameInteractionType Type { get; }
        public Dictionary<GameInteractionDataType, IGameInteractionData> Data { get; } = new();

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
                Data.Add(datum.DataType, datum);
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
                Data.Add(datum.DataType, datum);
            }
        }

        public GameInteraction(Action gameControllerMethod, GameInteractionType type)
            : this(gameControllerMethod, type, new()) { }

        public GameInteractionJson ToSerializable()
        {
            var dataJson = new List<IGameInteractionDataJson>();

            foreach (var data in Data.Values)
            {
                dataJson.Add(data.ToSerializable());
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
        public GameInteractionDataType DataType { get; }
        public IGameInteractionDataJson ToSerializable();
    }

    public record MulliganData : IGameInteractionData
    {
        public List<List<ICard>> Mulligans { get; }
        public IPlayer Player { get; }
        public GameInteractionDataType DataType => GameInteractionDataType.MulliganData;

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

        public GameInteractionDataType DataType => GameInteractionDataType.NumberData;

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
        public GameInteractionDataType DataType => GameInteractionDataType.TargetData;

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
        public GameInteractionDataType DataType => GameInteractionDataType.ConditionalTargetData;

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
        public GameInteractionDataType DataType => GameInteractionDataType.InteractionCardData;

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
        public GameInteractionDataType DataType => GameInteractionDataType.AttackData;

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
        public GameInteractionDataType DataType => GameInteractionDataType.WinnerData;

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
        public GameInteractionDataType DataType => GameInteractionDataType.SelectFromData;

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

    public enum GameInteractionDataType
    {
        MulliganData,
        NumberData,
        TargetData,
        ConditionalTargetData,
        InteractionCardData,
        AttackData,
        WinnerData,
        SelectFromData,
    }
}
