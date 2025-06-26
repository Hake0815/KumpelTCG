using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class SelectCardsGA : GameAction
    {
        public SelectCardsGA(
            IPlayerLogic player,
            int numberOfCards,
            ICardListLogic cardOptions,
            Predicate<ICardLogic> cardCondition,
            SelectedCardsOrigin origin
        )
        {
            Player = player;
            NumberOfCards = numberOfCards;
            CardOptions = cardOptions;
            CardCondition = cardCondition;
            Origin = origin;
        }

        public SelectCardsGA(
            IPlayerLogic player,
            int numberOfCards,
            ICardListLogic cardOptions,
            SelectedCardsOrigin origin
        )
        {
            Player = player;
            NumberOfCards = numberOfCards;
            CardOptions = cardOptions;
            Origin = origin;
        }

        [JsonConstructor]
        public SelectCardsGA(IPlayerLogic player, int numberOfCards, SelectedCardsOrigin origin)
        {
            Player = player;
            NumberOfCards = numberOfCards;
            Origin = origin;
        }

        public IPlayerLogic Player { get; }
        public int NumberOfCards { get; }

        [JsonIgnore]
        public ICardListLogic CardOptions { get; }
        public SelectedCardsOrigin Origin { get; }

        [JsonIgnore]
        public Predicate<ICardLogic> CardCondition { get; }
        public List<ICardLogic> SelectedCards { get; } = new();
        public List<ICardLogic> RemainingCards { get; } = new();

        internal enum SelectedCardsOrigin
        {
            Hand,
            Deck,
            DiscardPile,
            Other,
        }
    }
}
