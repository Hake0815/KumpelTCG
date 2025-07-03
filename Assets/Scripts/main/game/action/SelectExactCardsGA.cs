using System;
using System.Collections.Generic;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class SelectExactCardsGA : SelectCardsGA
    {
        public SelectExactCardsGA(
            IPlayerLogic player,
            int numberOfCards,
            ICardListLogic cardOptions,
            Predicate<ICardLogic> cardCondition,
            SelectedCardsOrigin origin
        )
            : base(player, origin)
        {
            NumberOfCards = numberOfCards;
            CardOptions = cardOptions;
            CardCondition = cardCondition;
        }

        public SelectExactCardsGA(
            IPlayerLogic player,
            int numberOfCards,
            ICardListLogic cardOptions,
            SelectedCardsOrigin origin
        )
            : base(player, origin)
        {
            NumberOfCards = numberOfCards;
            CardOptions = cardOptions;
        }

        [JsonConstructor]
        public SelectExactCardsGA(
            IPlayerLogic player,
            int numberOfCards,
            SelectedCardsOrigin origin
        )
            : base(player, origin)
        {
            NumberOfCards = numberOfCards;
        }

        public int NumberOfCards { get; }

        [JsonIgnore]
        public ICardListLogic CardOptions { get; }

        [JsonIgnore]
        public Predicate<ICardLogic> CardCondition { get; }
    }
}
