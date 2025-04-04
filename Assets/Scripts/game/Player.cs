using gamecore.card;

namespace gamecore.game
{
    public interface IPlayer
    {
        public string Name { get; }
        public IDeck Deck { get; }
        public IHand Hand { get; }
        public IDiscardPile DiscardPile { get; }
        public bool IsActive { get; }
        public IPokemonCard ActivePokemon { get; }
    }

    internal interface IPlayerLogic : IPlayer
    {
        internal new string Name { get; set; }
        internal new bool IsActive { get; set; }
        internal new IPokemonCard ActivePokemon { get; set; }
        internal new IDeckLogic Deck { get; }
        internal new IHandLogic Hand { get; }
        internal new IDiscardPileLogic DiscardPile { get; }
        internal void Draw(int amount);
    }

    internal class Player : IPlayerLogic
    {
        private IDeckLogic _deck;
        private IHandLogic _hand = new Hand();
        private IPokemonCard _activePokemon;
        private IDiscardPileLogic _discardPile = new DiscardPile();
        private bool _isActive = false;
        private string _name;

        // Explicit interface implementations for IPlayer
        string IPlayer.Name => _name;
        IDeck IPlayer.Deck => _deck;
        IHand IPlayer.Hand => _hand;
        IDiscardPile IPlayer.DiscardPile => _discardPile;
        bool IPlayer.IsActive => _isActive;
        IPokemonCard IPlayer.ActivePokemon => _activePokemon;

        // Explicit interface implementations for IPlayerLogic
        string IPlayerLogic.Name
        {
            get => _name;
            set => _name = value;
        }
        bool IPlayerLogic.IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }
        IPokemonCard IPlayerLogic.ActivePokemon
        {
            get => _activePokemon;
            set => _activePokemon = value;
        }
        IDeckLogic IPlayerLogic.Deck => _deck;
        IHandLogic IPlayerLogic.Hand => _hand;
        IDiscardPileLogic IPlayerLogic.DiscardPile => _discardPile;

        internal Player(IDeckLogic deck)
        {
            _deck = deck;
        }

        void IPlayerLogic.Draw(int amount)
        {
            var drawnCards = _deck.Draw(amount);
            if (drawnCards != null)
            {
                _hand.AddCards(drawnCards);
            }
        }
    }
}
