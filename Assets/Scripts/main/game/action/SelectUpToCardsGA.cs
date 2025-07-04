using System;
using gamecore.card;
using Newtonsoft.Json;

namespace gamecore.game.action
{
    class SelectUpToCardsGA : SelectCardsGA
    {
        public SelectUpToCardsGA(
            IPlayerLogic player,
            int amount,
            ICardListLogic cardOptions,
            Predicate<ICardLogic> cardCondition,
            SelectedCardsOrigin origin
        )
            : base(player, origin)
        {
            Amount = amount;
            CardOptions = cardOptions;
            CardCondition = cardCondition;
        }

        [JsonConstructor]
        public SelectUpToCardsGA(IPlayerLogic player, int amount, SelectedCardsOrigin origin)
            : base(player, origin)
        {
            Amount = amount;
        }

        public int Amount { get; }

        [JsonIgnore]
        public ICardListLogic CardOptions { get; }

        [JsonIgnore]
        public Predicate<ICardLogic> CardCondition { get; }
    }
}
