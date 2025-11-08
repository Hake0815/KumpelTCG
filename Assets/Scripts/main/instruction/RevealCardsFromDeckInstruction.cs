using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using gamecore.instruction.filter;
using gamecore.serialization;

namespace gamecore.instruction
{
    class RevealCardsFromDeckInstruction : IInstruction
    {
        public int Count { get; }

        public RevealCardsFromDeckInstruction(int cardsToReveal)
        {
            Count = cardsToReveal;
        }

        public void Perform(ICardLogic card, ActionSystem actionSystem)
        {
            actionSystem.AddReaction(new RevealCardsFromDeckGA(card.Owner, Count));
        }

        public InstructionJson ToSerializable()
        {
            return new InstructionJson(
                instructionType: InstructionType.RevealCards,
                data: new()
                {
                    new CardAmountInstructionDataJson(
                        new IntRange(Count, Count),
                        CardPosition.Deck
                    ),
                }
            );
        }
    }
}
