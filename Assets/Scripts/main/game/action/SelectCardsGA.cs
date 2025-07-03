using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    abstract class SelectCardsGA : GameAction
    {
        protected SelectCardsGA(IPlayerLogic player, SelectedCardsOrigin origin)
        {
            Player = player;
            Origin = origin;
        }

        public IPlayerLogic Player { get; }

        public SelectedCardsOrigin Origin { get; }

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

    class SelectCardsEntry
    {
        public SelectCardsEntry(
            Predicate<ICardLogic> cardCondition,
            Predicate<int> cardCountCondition
        )
        {
            CardCondition = cardCondition;
            CardCountCondition = cardCountCondition;
        }

        public Predicate<ICardLogic> CardCondition { get; }
        public Predicate<int> CardCountCondition { get; }
    }
}
