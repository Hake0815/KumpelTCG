using System;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class QuickSelectCardsGA : SelectCardsGA
    {
        public QuickSelectCardsGA(
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

        public QuickSelectCardsGA(
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
        public QuickSelectCardsGA(
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
