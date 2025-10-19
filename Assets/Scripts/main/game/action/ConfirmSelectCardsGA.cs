using System;
using System.Collections.Generic;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class ConfirmSelectCardsGA : SelectCardsGA
    {
        public ConfirmSelectCardsGA(
            IPlayerLogic player,
            Predicate<int> numberofcardscondition,
            List<ICardLogic> cardOptions,
            Predicate<ICardLogic> cardCondition,
            SelectedCardsOrigin origin,
            string selectionId
        )
            : base(player, origin, selectionId)
        {
            NumberOfCardsCondition = numberofcardscondition;
            CardOptions = new(cardOptions);
            CardCondition = cardCondition;
        }

        [JsonConstructor]
        public ConfirmSelectCardsGA(
            IPlayerLogic player,
            SelectedCardsOrigin origin,
            string selectionId
        )
            : base(player, origin, selectionId) { }

        [JsonIgnore]
        public Predicate<int> NumberOfCardsCondition { get; }

        [JsonIgnore]
        public List<ICardLogic> CardOptions { get; }

        [JsonIgnore]
        public Predicate<ICardLogic> CardCondition { get; }
    }
}
