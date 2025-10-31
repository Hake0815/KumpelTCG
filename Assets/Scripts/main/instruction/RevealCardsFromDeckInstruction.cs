using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;

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
                data: new() { { "count", Count }, { "from", "deck" } }
            );
        }
    }
}
