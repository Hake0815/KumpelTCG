using System;
using gamecore.instruction;
using gamecore.instruction.filter;

namespace gamecore.serialization
{
    [Serializable]
    public class InstructionDataJson : IJsonStringSerializable, IInstructionDataPayload
    {
        public InstructionDataType InstructionDataType { get; }
        public IInstructionDataPayload Payload { get; }

        public InstructionDataJson(
            InstructionDataType instructionDataType,
            IInstructionDataPayload payload
        )
        {
            InstructionDataType = instructionDataType;
            Payload = payload;
        }
    }

    public interface IInstructionDataPayload { }

    [Serializable]
    public class AttackInstructionDataJson : IInstructionDataPayload
    {
        public AttackTarget AttackTarget { get; }
        public int Damage { get; }

        public AttackInstructionDataJson(AttackTarget attackTarget, int damage)
        {
            AttackTarget = attackTarget;
            Damage = damage;
        }
    }

    [Serializable]
    public class DiscardInstructionDataJson : IInstructionDataPayload
    {
        public TargetSource TargetSource { get; }

        public DiscardInstructionDataJson(TargetSource targetSource)
        {
            TargetSource = targetSource;
        }
    }

    [Serializable]
    public class CardAmountInstructionDataJson : IInstructionDataPayload
    {
        public IntRange Amount { get; }
        public CardPosition FromPosition { get; }

        public CardAmountInstructionDataJson(IntRange amount, CardPosition fromPosition)
        {
            Amount = amount;
            FromPosition = fromPosition;
        }
    }

    [Serializable]
    public class ReturnToDeckTypeInstructionDataJson : IInstructionDataPayload
    {
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
    public class FilterInstructionDataJson : IInstructionDataPayload
    {
        public FilterJson Filter { get; }

        public FilterInstructionDataJson(FilterJson filter)
        {
            Filter = filter;
        }
    }

    [Serializable]
    public class PlayerTargetInstructionDataJson : IInstructionDataPayload
    {
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
