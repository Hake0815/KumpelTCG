using System;
using System.Collections.Generic;
using gamecore.card;
using gamecore.game.interaction;

namespace gamecore.serialization
{
    [Serializable]
    public class GameInteractionJson : IJsonStringSerializable
    {
        public GameInteractionType Type { get; }
        public List<IGameInteractionDataJson> Data { get; }

        public GameInteractionJson(GameInteractionType type, List<IGameInteractionDataJson> data)
        {
            Type = type;
            Data = data ?? new();
        }
    }

    public interface IGameInteractionDataJson
    {
        GameInteractionDataType DataType { get; }
    }

    [Serializable]
    public class MulliganDataJson : IGameInteractionDataJson
    {
        public List<List<CardJson>> Mulligans { get; }
        public PlayerStateJson Player { get; }
        public GameInteractionDataType DataType => GameInteractionDataType.MulliganData;

        public MulliganDataJson(List<List<CardJson>> mulligans, PlayerStateJson player)
        {
            Mulligans = mulligans ?? new List<List<CardJson>>();
            Player = player ?? throw new ArgumentNullException(nameof(player));
        }
    }

    [Serializable]
    public class NumberDataJson : IGameInteractionDataJson
    {
        public int Number { get; }
        public GameInteractionDataType DataType => GameInteractionDataType.NumberData;

        public NumberDataJson(int number)
        {
            Number = number;
        }
    }

    [Serializable]
    public class TargetDataJson : IGameInteractionDataJson
    {
        public GameInteractionDataType DataType => GameInteractionDataType.TargetData;
        public List<CardJson> PossibleTargets { get; }
        public int NumberOfTargets { get; }
        public ActionOnSelection TargetAction { get; }
        public ActionOnSelection RemainderAction { get; }

        public TargetDataJson(
            int numberOfTargets,
            List<CardJson> possibleTargets,
            ActionOnSelection targetAction,
            ActionOnSelection remainderAction
        )
        {
            NumberOfTargets = numberOfTargets;
            PossibleTargets = possibleTargets ?? new List<CardJson>();
            TargetAction = targetAction;
            RemainderAction = remainderAction;
        }
    }

    [Serializable]
    public class ConditionalTargetDataJson : IGameInteractionDataJson
    {
        public GameInteractionDataType DataType => GameInteractionDataType.ConditionalTargetData;
        public List<CardJson> PossibleTargets { get; }
        public ConditionalTargetQueryJson ConditionalTargetQuery { get; }
        public ActionOnSelection TargetAction { get; }
        public ActionOnSelection RemainderAction { get; }

        public ConditionalTargetDataJson(
            List<CardJson> possibleTargets,
            ConditionalTargetQueryJson conditionalTargetQuery,
            ActionOnSelection targetAction,
            ActionOnSelection remainderAction
        )
        {
            PossibleTargets = possibleTargets;
            ConditionalTargetQuery = conditionalTargetQuery;
            TargetAction = targetAction;
            RemainderAction = remainderAction;
        }
    }

    [Serializable]
    public class InteractionCardDataJson : IGameInteractionDataJson
    {
        public CardJson Card { get; }
        public GameInteractionDataType DataType => GameInteractionDataType.InteractionCardData;

        public InteractionCardDataJson(CardJson card)
        {
            Card = card ?? throw new ArgumentNullException(nameof(card));
        }
    }

    [Serializable]
    public class AttackDataJson : IGameInteractionDataJson
    {
        public AttackJson Attack { get; }
        public GameInteractionDataType DataType => GameInteractionDataType.AttackData;

        public AttackDataJson(AttackJson attack)
        {
            Attack = attack ?? throw new ArgumentNullException(nameof(attack));
        }
    }

    [Serializable]
    public class WinnerDataJson : IGameInteractionDataJson
    {
        public PlayerStateJson Winner { get; }
        public string Message { get; }
        public GameInteractionDataType DataType => GameInteractionDataType.WinnerData;

        public WinnerDataJson(PlayerStateJson winner, string message)
        {
            Winner = winner ?? throw new ArgumentNullException(nameof(winner));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }

    [Serializable]
    public class SelectFromDataJson : IGameInteractionDataJson
    {
        public SelectFrom SelectFrom { get; }
        public List<CardJson> SelectionSource { get; }
        public GameInteractionDataType DataType => GameInteractionDataType.SelectFromData;

        public SelectFromDataJson(SelectFrom selectFrom, List<CardJson> selectionSource)
        {
            SelectFrom = selectFrom;
            SelectionSource = selectionSource ?? new List<CardJson>();
        }

        public SelectFromDataJson(SelectFrom selectFrom)
        {
            SelectFrom = selectFrom;
            SelectionSource = new List<CardJson>();
        }
    }
}
