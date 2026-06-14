using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamecore.card;
using gamecore.serialization;
using Google.Protobuf;

namespace gamecore.game.interaction
{
    public record GameInteraction
    {
        public Action GameControllerMethod { get; }
        public Action<List<ICard>> GameControllerMethodWithTargets { get; }
        public GameInteractionType Type { get; }
        public Dictionary<GameInteractionDataType, IGameInteractionData> Data { get; } = new();

        public GameInteraction(
            Func<List<ICard>, Task> gameControllerMethodWithTargets,
            GameInteractionType type,
            List<IGameInteractionData> data
        )
        {
            GameControllerMethodWithTargets = targets =>
                gameControllerMethodWithTargets(targets).GetAwaiter().GetResult();
            Type = type;
            foreach (var datum in data)
            {
                Data.Add(datum.DataType, datum);
            }
        }

        public GameInteraction(
            Func<Task> gameControllerMethod,
            GameInteractionType type,
            List<IGameInteractionData> data
        )
        {
            GameControllerMethod = () => gameControllerMethod().GetAwaiter().GetResult();
            Type = type;
            foreach (var datum in data)
            {
                Data.Add(datum.DataType, datum);
            }
        }

        public GameInteraction(Func<Task> gameControllerMethod, GameInteractionType type)
            : this(gameControllerMethod, type, new()) { }

        public ProtoBufGameInteraction ToSerializable()
        {
            var protoBufData = new List<ProtoBufGameInteractionData>();

            foreach (var data in Data.Values)
            {
                protoBufData.Add(data.ToSerializable());
            }

            if (Type == GameInteractionType.PerformAbility)
            {
                var abilityData = new ProtoBufAbilityData
                {
                    Ability = (
                        (Data[GameInteractionDataType.InteractionCardData] as InteractionCard).Card
                        as IPokemonCardLogic
                    ).Ability.ToSerializable(),
                };

                protoBufData.Add(
                    new ProtoBufGameInteractionData
                    {
                        DataType =
                            ProtoBufGameInteractionDataType.GameInteractionDataTypeAbilityData,
                        AbilityData = abilityData,
                    }
                );
            }

            return new ProtoBufGameInteraction
            {
                Type = (ProtoBufGameInteractionType)Type,
                Data = { protoBufData },
            };
        }

        public byte[] ToByteArray()
        {
            return ToSerializable().ToByteArray();
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
        public ProtoBufGameInteractionData ToSerializable();
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

        public ProtoBufGameInteractionData ToSerializable()
        {
            return new ProtoBufGameInteractionData
            {
                DataType = DataType.ToProtobuf(),
                MulliganData = new ProtoBufMulliganData
                {
                    Mulligans =
                    {
                        Mulligans.Select(mulligan => new ProtoBufCardList
                        {
                            Cards = { mulligan.Select(card => card.DeckId) },
                        }),
                    },
                    Player = ((IPlayerLogic)Player).ToSerializable(),
                },
            };
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

        public ProtoBufGameInteractionData ToSerializable()
        {
            return new ProtoBufGameInteractionData
            {
                DataType = DataType.ToProtobuf(),
                NumberData = new ProtoBufNumberData { Number = Number },
            };
        }
    }

    public record TargetData : IGameInteractionData
    {
        public GameInteractionDataType DataType => GameInteractionDataType.TargetData;

        public TargetData(
            int numberOfTargets,
            List<ICard> possibleTargets,
            ActionOnSelection targetAction,
            ActionOnSelection remainderAction,
            bool allowMultipleTimes = false
        )
        {
            NumberOfTargets = numberOfTargets;
            PossibleTargets = possibleTargets;
            TargetAction = targetAction;
            RemainderAction = remainderAction;
            AllowMultipleTimes = allowMultipleTimes;
        }

        public List<ICard> PossibleTargets { get; }
        public int NumberOfTargets { get; } = 0;
        public bool AllowMultipleTimes { get; }
        public ActionOnSelection TargetAction { get; }
        public ActionOnSelection RemainderAction { get; }

        public ProtoBufGameInteractionData ToSerializable()
        {
            var protoBufGameInteractionData = new ProtoBufGameInteractionData
            {
                DataType = DataType.ToProtobuf(),
                TargetData = new ProtoBufTargetData
                {
                    AllowMultipleTimes = AllowMultipleTimes,
                    NumberOfTargets = NumberOfTargets,
                    TargetAction = TargetAction.ToProtoBuf(),
                    RemainderAction = RemainderAction.ToProtoBuf(),
                },
            };
            protoBufGameInteractionData.TargetData.PossibleTargets.Capacity = PossibleTargets.Count;
            foreach (var card in PossibleTargets)
            {
                protoBufGameInteractionData.TargetData.PossibleTargets.Add(card.DeckId);
            }
            return protoBufGameInteractionData;
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
            bool allowMultipleTimes = false,
            bool isQuickSelection = true
        )
        {
            ConditionalTargetQuery = conditionalTargetQuery;
            PossibleTargets = possibleTargets;
            AllowMultipleTimes = allowMultipleTimes;
            IsQuickSelection = isQuickSelection;
            TargetAction = targetAction;
            RemainderAction = remainderAction;
        }

        public List<ICard> PossibleTargets { get; }
        public IConditionalTargetQuery ConditionalTargetQuery { get; }
        public bool AllowMultipleTimes { get; }
        public bool IsQuickSelection { get; }
        public ActionOnSelection TargetAction { get; }
        public ActionOnSelection RemainderAction { get; }

        public ProtoBufGameInteractionData ToSerializable()
        {
            var protoBufGameInteractionData = new ProtoBufGameInteractionData
            {
                DataType = DataType.ToProtobuf(),
                TargetData = new ProtoBufTargetData
                {
                    AllowMultipleTimes = AllowMultipleTimes,
                    ConditionalTargetQuery = ConditionalTargetQuery.ToSerializable(),
                    TargetAction = TargetAction.ToProtoBuf(),
                    RemainderAction = RemainderAction.ToProtoBuf(),
                },
            };
            protoBufGameInteractionData.TargetData.PossibleTargets.Capacity = PossibleTargets.Count;
            foreach (var card in PossibleTargets)
            {
                protoBufGameInteractionData.TargetData.PossibleTargets.Add(card.DeckId);
            }
            return protoBufGameInteractionData;
        }

        public List<ICard> GetCandidatesGivenPartialSelection(List<ICard> partialSelection)
        {
            if (AllowMultipleTimes)
            {
                return PossibleTargets.Where(candidateCard => ConditionalTargetQuery.CanBeAddedToPartialSelection(candidateCard, partialSelection)).ToList();
            }
            return PossibleTargets.Where(candidateCard => !partialSelection.Select(card => card.DeckId).Contains(candidateCard.DeckId))
                .Where(candidateCard => ConditionalTargetQuery.CanBeAddedToPartialSelection(candidateCard, partialSelection))
                .ToList();
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

        public ProtoBufGameInteractionData ToSerializable()
        {
            return new ProtoBufGameInteractionData
            {
                DataType = DataType.ToProtobuf(),
                InteractionCardData = new ProtoBufInteractionCardData { Card = Card.DeckId },
            };
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

        public ProtoBufGameInteractionData ToSerializable()
        {
            return new ProtoBufGameInteractionData
            {
                DataType = DataType.ToProtobuf(),
                AttackData = new ProtoBufAttackData { Attack = Attack.ToSerializable() },
            };
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

        public ProtoBufGameInteractionData ToSerializable()
        {
            return new ProtoBufGameInteractionData
            {
                DataType = DataType.ToProtobuf(),
                WinnerData = new ProtoBufWinnerData
                {
                    Winner = ((IPlayerLogic)Winner).ToSerializable(),
                    Message = Message,
                },
            };
        }
    }

    public record SelectFromData : IGameInteractionData
    {
        public SelectFrom SelectFrom { get; }
        public List<ICard> SelectionSource { get; }
        public GameInteractionDataType DataType => GameInteractionDataType.SelectFromData;

        public SelectFromData(SelectFrom selectFrom, List<ICard> selectionSource)
        {
            SelectFrom = selectFrom;
            SelectionSource = selectionSource;
        }

        public SelectFromData(SelectFrom selectFrom)
        {
            SelectFrom = selectFrom;
        }

        public ProtoBufGameInteractionData ToSerializable()
        {
            return new ProtoBufGameInteractionData
            {
                DataType = DataType.ToProtobuf(),
                SelectFromData = new ProtoBufSelectFromData
                {
                    SelectFrom = SelectFrom.ToProtoBuf(),
                },
            };
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
