using System;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class ConfirmSelectCardsGA : SelectCardsGA
    {
        public ConfirmSelectCardsGA(
            IPlayerLogic player,
            Predicate<int> numberofcardscondition,
            ICardListLogic cardOptions,
            Predicate<ICardLogic> cardCondition,
            SelectedCardsOrigin origin
        )
            : base(player, origin)
        {
            NumberOfCardsCondition = numberofcardscondition;
            CardOptions = cardOptions;
            CardCondition = cardCondition;
        }

        [JsonConstructor]
        public ConfirmSelectCardsGA(IPlayerLogic player, SelectedCardsOrigin origin)
            : base(player, origin) { }

        [JsonIgnore]
        public Predicate<int> NumberOfCardsCondition { get; }

        [JsonIgnore]
        public ICardListLogic CardOptions { get; }

        [JsonIgnore]
        public Predicate<ICardLogic> CardCondition { get; }
    }
}
