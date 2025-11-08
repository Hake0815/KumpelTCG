using System;
using System.Collections.Generic;
using gamecore.card;
using gamecore.common;

namespace gamecore.game.interaction
{
    [Serializable]
    public class GameInteractionJson : JsonStringSerializable
    {
        public GameInteractionType Type { get; }
        public Dictionary<string, IGameInteractionDataJson> Data { get; }

        public GameInteractionJson(
            GameInteractionType type,
            Dictionary<string, IGameInteractionDataJson> data
        )
        {
            Type = type;
            Data = data ?? new Dictionary<string, IGameInteractionDataJson>();
        }
    }

    public interface IGameInteractionDataJson
    {
        string Name { get; }
    }

    [Serializable]
    public class MulliganDataJson : IGameInteractionDataJson
    {
        public List<List<CardJson>> Mulligans { get; }
        public PlayerStateJson Player { get; }
        public string Name => MulliganData.NAME;

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
        public string Name => NumberData.NAME;

        public NumberDataJson(int number)
        {
            Number = number;
        }
    }

    [Serializable]
    public class TargetDataJson : IGameInteractionDataJson
    {
        public string Name => TargetData.NAME;
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
        public string Name => ConditionalTargetData.NAME;
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
        public string Name => InteractionCard.NAME;

        public InteractionCardDataJson(CardJson card)
        {
            Card = card ?? throw new ArgumentNullException(nameof(card));
        }
    }

    [Serializable]
    public class AttackDataJson : IGameInteractionDataJson
    {
        public AttackJson Attack { get; }
        public string Name => AttackData.NAME;

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
        public string Name => WinnerData.NAME;

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
        public string Name => SelectFromData.NAME;

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
