using System;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.interaction;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    abstract class SelectCardsGA : GameAction
    {
        protected SelectCardsGA(
            IPlayerLogic player,
            SelectedCardsOrigin origin,
            string selectionId,
            ActionOnSelection targetAction,
            ActionOnSelection remainderAction,
            List<ICardLogic> selectedCards = null,
            List<ICardLogic> remainingCards = null
        )
        {
            Player = player;
            Origin = origin;
            SelectionId = selectionId;
            TargetAction = targetAction;
            RemainderAction = remainderAction;
            SelectedCards = selectedCards is null ? new() : new(selectedCards);
            RemainingCards = remainingCards is null ? new() : new(remainingCards);
        }

        public IPlayerLogic Player { get; }

        public SelectedCardsOrigin Origin { get; }
        public string SelectionId { get; }
        public List<ICardLogic> SelectedCards { get; }
        public List<ICardLogic> RemainingCards { get; }
        public ActionOnSelection TargetAction { get; }
        public ActionOnSelection RemainderAction { get; }

        internal enum SelectedCardsOrigin
        {
            Hand,
            Deck,
            DiscardPile,
            Floating,
        }
    }
}
