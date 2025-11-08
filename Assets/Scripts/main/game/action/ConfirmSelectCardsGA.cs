using System;
using System.Collections.Generic;
using gamecore.card;
using gamecore.game.interaction;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class ConfirmSelectCardsGA : SelectCardsGA
    {
        public ConfirmSelectCardsGA(
            IPlayerLogic player,
            ConditionalTargetQuery numberOfCardsCondition,
            List<ICardLogic> cardOptionSource,
            Predicate<ICardLogic> cardCondition,
            SelectedCardsOrigin origin,
            string selectionId,
            ActionOnSelection targetAction,
            ActionOnSelection remainderAction
        )
            : base(player, origin, selectionId, targetAction, remainderAction)
        {
            NumberOfCardsCondition = numberOfCardsCondition;
            CardOptionSource = new(cardOptionSource);
            CardCondition = cardCondition;
        }

        [JsonConstructor]
        public ConfirmSelectCardsGA(
            IPlayerLogic player,
            SelectedCardsOrigin origin,
            string selectionId,
            ActionOnSelection targetAction,
            ActionOnSelection remainderAction,
            List<ICardLogic> selectedCards,
            List<ICardLogic> remainingCards
        )
            : base(
                player,
                origin,
                selectionId,
                targetAction,
                remainderAction,
                selectedCards,
                remainingCards
            ) { }

        [JsonIgnore]
        public ConditionalTargetQuery NumberOfCardsCondition { get; }

        [JsonIgnore]
        public List<ICardLogic> CardOptionSource { get; }

        [JsonIgnore]
        public Predicate<ICardLogic> CardCondition { get; }
    }
}
