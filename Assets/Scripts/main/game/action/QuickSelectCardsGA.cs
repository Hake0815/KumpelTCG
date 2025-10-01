using System;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class QuickSelectCardsGA : SelectCardsGA
    {
        public QuickSelectCardsGA(
            IPlayerLogic player,
            Predicate<int> numberofcardscondition,
            CardListLogicAbstract cardOptions,
            Predicate<ICardLogic> cardCondition,
            SelectedCardsOrigin origin,
            string selectionId
        )
            : base(player, origin, selectionId)
        {
            NumberOfCardsCondition = numberofcardscondition;
            CardOptions = cardOptions;
            CardCondition = cardCondition;
        }

        [JsonConstructor]
        public QuickSelectCardsGA(
            IPlayerLogic player,
            SelectedCardsOrigin origin,
            string selectionId
        )
            : base(player, origin, selectionId) { }

        [JsonIgnore]
        public Predicate<int> NumberOfCardsCondition { get; }

        [JsonIgnore]
        public CardListLogicAbstract CardOptions { get; }

        [JsonIgnore]
        public Predicate<ICardLogic> CardCondition { get; }
    }
}
