using gamecore.actionsystem;
using gamecore.effect;
using gamecore.game;
using gamecore.game.action;
using Newtonsoft.Json;

namespace gamecore.card
{
    class SupporterCard : TrainerCard
    {
        public const string PLAYED_SUPPORTER_THIS_TURN = "playedSupporterThisTurn";

        public override CardSubtype CardSubtype => CardSubtype.Supporter;

        public SupporterCard(ITrainerCardData cardData, IPlayerLogic owner, int deckId)
            : base(cardData, owner, deckId) { }

        [JsonConstructor]
        public SupporterCard(string name, string id, int deckId, IPlayerLogic owner)
            : base(name, id, deckId, owner) { }

        public override bool IsSupporterCard() => true;

        public override bool IsItemCard() => false;

        public override void Play(ActionSystem actionSystem)
        {
            actionSystem.AddReaction(new PlaySupporterGA(Owner));
            base.Play(actionSystem);
        }

        public override bool IsPlayable()
        {
            if (
                Owner.PerformedOncePerTurnActions.Contains(PLAYED_SUPPORTER_THIS_TURN)
                || Owner.HasEffect<FirstTurnOfGameEffect>()
            )
                return false;
            return base.IsPlayable();
        }
    }
}
