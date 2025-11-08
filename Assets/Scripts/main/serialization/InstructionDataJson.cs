using System;
using gamecore.instruction;
using gamecore.instruction.filter;

namespace gamecore.serialization
{
    public interface IInstructionDataJson : IJsonStringSerializable
    {
        InstructionDataType InstructionDataType { get; }
    }

    [Serializable]
    public class AttackInstructionDataJson : IInstructionDataJson
    {
        public InstructionDataType InstructionDataType => InstructionDataType.AttackData;
        public AttackTarget AttackTarget { get; }
        public int Damage { get; }

        public AttackInstructionDataJson(AttackTarget attackTarget, int damage)
        {
            AttackTarget = attackTarget;
            Damage = damage;
        }
    }

    [Serializable]
    public class DiscardInstructionDataJson : IInstructionDataJson
    {
        public InstructionDataType InstructionDataType => InstructionDataType.DiscardData;
        public TargetSource TargetSource { get; }

        public DiscardInstructionDataJson(TargetSource targetSource)
        {
            TargetSource = targetSource;
        }
    }

    [Serializable]
    public class CardAmountInstructionDataJson : IInstructionDataJson
    {
        public InstructionDataType InstructionDataType => InstructionDataType.CardAmountData;
        public IntRange Amount { get; }
        public CardPosition FromPosition { get; }

        public CardAmountInstructionDataJson(IntRange amount, CardPosition fromPosition)
        {
            Amount = amount;
            FromPosition = fromPosition;
        }
    }

    [Serializable]
    public class ReturnToDeckTypeInstructionDataJson : IInstructionDataJson
    {
        public InstructionDataType InstructionDataType => InstructionDataType.ReturnToDeckTypeData;
        public ReturnToDeckType ReturnToDeckType { get; }
        public CardPosition FromPosition { get; }

        public ReturnToDeckTypeInstructionDataJson(
            ReturnToDeckType returnToDeckType,
            CardPosition fromPosition
        )
        {
            ReturnToDeckType = returnToDeckType;
            FromPosition = fromPosition;
        }
    }

    [Serializable]
    public class FilterInstructionDataJson : IInstructionDataJson
    {
        public InstructionDataType InstructionDataType => InstructionDataType.FilterData;
        public FilterJson Filter { get; }

        public FilterInstructionDataJson(FilterJson filter)
        {
            Filter = filter;
        }
    }

    [Serializable]
    public class PlayerTargetInstructionDataJson : IInstructionDataJson
    {
        public InstructionDataType InstructionDataType => InstructionDataType.PlayerTargetData;
        public PlayerTarget PlayerTarget { get; }

        public PlayerTargetInstructionDataJson(PlayerTarget playerTarget)
        {
            PlayerTarget = playerTarget;
        }
    }

    public enum InstructionDataType
    {
        AttackData,
        DiscardData,
        CardAmountData,
        ReturnToDeckTypeData,
        FilterData,
        PlayerTargetData,
    }
}
