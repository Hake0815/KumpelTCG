using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    abstract class SelectCardsGA : GameAction
    {
        protected SelectCardsGA(IPlayerLogic player, SelectedCardsOrigin origin, string selectionId)
        {
            Player = player;
            Origin = origin;
            SelectionId = selectionId;
        }

        public IPlayerLogic Player { get; }

        public SelectedCardsOrigin Origin { get; }
        public string SelectionId { get; }
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
