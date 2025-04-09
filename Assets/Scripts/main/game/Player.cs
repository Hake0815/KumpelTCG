using gamecore.card;

namespace gamecore.game
{
    public interface IPlayer
    {
        string Name { get; }
        IDeck Deck { get; }
        IHand Hand { get; }
        IDiscardPile DiscardPile { get; }
        bool IsActive { get; }
        IPokemonCard ActivePokemon { get; }
    }

    internal interface IPlayerLogic : IPlayer
    {
        new string Name { get; set; }
        new bool IsActive { get; set; }
        new IPokemonCard ActivePokemon { get; set; }
        new IDeckLogic Deck { get; set; }
        new IHandLogic Hand { get; }
        new IDiscardPileLogic DiscardPile { get; }
        void Draw(int amount);
    }

    internal class Player : IPlayerLogic
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public IPokemonCard ActivePokemon { get; set; }
        public IDeckLogic Deck { get; set; }
        public IHandLogic Hand { get; } = new Hand();
        public IDiscardPileLogic DiscardPile { get; } = new DiscardPile();

        // Explicit interface implementations for IPlayer
        IDeck IPlayer.Deck => Deck;
        IHand IPlayer.Hand => Hand;
        IDiscardPile IPlayer.DiscardPile => DiscardPile;

        public void Draw(int amount)
        {
            var drawnCards = Deck.Draw(amount);
            if (drawnCards != null)
            {
                Hand.AddCards(drawnCards);
            }
        }
    }
}
