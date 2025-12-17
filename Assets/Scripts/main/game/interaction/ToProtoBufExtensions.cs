using System;
using gamecore.serialization;

namespace gamecore.game.interaction
{
    public static class ToProtoBufExtensions
    {
        public static ProtoBufGameInteractionDataType ToProtobuf(
            this GameInteractionDataType dataType
        )
        {
            return dataType switch
            {
                GameInteractionDataType.MulliganData =>
                    ProtoBufGameInteractionDataType.GameInteractionDataTypeMulliganData,
                GameInteractionDataType.NumberData =>
                    ProtoBufGameInteractionDataType.GameInteractionDataTypeNumberData,
                GameInteractionDataType.TargetData =>
                    ProtoBufGameInteractionDataType.GameInteractionDataTypeTargetData,
                GameInteractionDataType.ConditionalTargetData =>
                    ProtoBufGameInteractionDataType.GameInteractionDataTypeConditionalTargetData,
                GameInteractionDataType.InteractionCardData =>
                    ProtoBufGameInteractionDataType.GameInteractionDataTypeInteractionCardData,
                GameInteractionDataType.AttackData =>
                    ProtoBufGameInteractionDataType.GameInteractionDataTypeAttackData,
                GameInteractionDataType.WinnerData =>
                    ProtoBufGameInteractionDataType.GameInteractionDataTypeWinnerData,
                GameInteractionDataType.SelectFromData =>
                    ProtoBufGameInteractionDataType.GameInteractionDataTypeSelectFromData,
                _ => throw new InvalidOperationException(
                    $"Invalid game interaction data type: {dataType}"
                ),
            };
            ;
        }

        public static ProtoBufActionOnSelection ToProtoBuf(this ActionOnSelection actionOnSelection)
        {
            return actionOnSelection switch
            {
                ActionOnSelection.Discard => ProtoBufActionOnSelection.ActionOnSelectionDiscard,
                ActionOnSelection.TakeToHand =>
                    ProtoBufActionOnSelection.ActionOnSelectionTakeToHand,
                ActionOnSelection.Evolve => ProtoBufActionOnSelection.ActionOnSelectionEvolve,
                ActionOnSelection.AttachTo => ProtoBufActionOnSelection.ActionOnSelectionAttachTo,
                ActionOnSelection.Promote => ProtoBufActionOnSelection.ActionOnSelectionPromote,
                ActionOnSelection.Nothing => ProtoBufActionOnSelection.ActionOnSelectionNothing,
                ActionOnSelection.PutUnderDeck =>
                    ProtoBufActionOnSelection.ActionOnSelectionPutUnderDeck,
                _ => throw new InvalidOperationException(
                    $"Invalid action on selection: {actionOnSelection}"
                ),
            };
        }

        public static ProtoBufSelectFrom ToProtoBuf(this SelectFrom selectFrom)
        {
            return selectFrom switch
            {
                SelectFrom.InPlay => ProtoBufSelectFrom.SelectFromInPlay,
                SelectFrom.Floating => ProtoBufSelectFrom.SelectFromFloating,
                SelectFrom.Deck => ProtoBufSelectFrom.SelectFromDeck,
                SelectFrom.DiscardPile => ProtoBufSelectFrom.SelectFromDiscardPile,
                _ => throw new InvalidOperationException($"Invalid select from: {selectFrom}"),
            };
        }

        public static ProtoBufLogicalQueryOperator ToProtoBuf(
            this LogicalQueryOperator logicalQueryOperator
        )
        {
            return logicalQueryOperator switch
            {
                LogicalQueryOperator.And => ProtoBufLogicalQueryOperator.LogicalQueryOperatorAnd,
                LogicalQueryOperator.Or => ProtoBufLogicalQueryOperator.LogicalQueryOperatorOr,
                _ => throw new InvalidOperationException(
                    $"Unknown logical operator: {logicalQueryOperator}"
                ),
            };
        }

        public static ProtoBufSelectionQualifier ToProtoBuf(
            this SelectionQualifier selectionQualifier
        )
        {
            return selectionQualifier switch
            {
                SelectionQualifier.NumberOfCards =>
                    ProtoBufSelectionQualifier.SelectionQualifierNumberOfCards,
                SelectionQualifier.ProvidedEnergy =>
                    ProtoBufSelectionQualifier.SelectionQualifierProvidedEnergy,
                _ => throw new InvalidOperationException(
                    $"Unknown selection qualifier: {selectionQualifier}"
                ),
            };
        }
    }
}
