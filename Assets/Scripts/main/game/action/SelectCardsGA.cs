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
            ActionOnSelection remainderAction
        )
        {
            Player = player;
            Origin = origin;
            SelectionId = selectionId;
            TargetAction = targetAction;
            RemainderAction = remainderAction;
        }

        public IPlayerLogic Player { get; }

        public SelectedCardsOrigin Origin { get; }
        public string SelectionId { get; }
        public List<ICardLogic> SelectedCards { get; } = new();
        public List<ICardLogic> RemainingCards { get; } = new();
        public ActionOnSelection TargetAction { get; }
        public ActionOnSelection RemainderAction { get; }

        internal enum SelectedCardsOrigin
        {
            Hand,
            Deck,
            DiscardPile,
            Other,
        }
    }
}
