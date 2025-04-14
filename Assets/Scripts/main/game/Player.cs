using gamecore.card;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering.VirtualTexturing;

namespace gamecore.game
{
    public interface IPlayer
    {
        string Name { get; }
        IDeck Deck { get; }
        ICardList Hand { get; }
        IBench Bench { get; }
        IDiscardPile DiscardPile { get; }
        IPrizes Prizes { get; }
        bool IsActive { get; }
        IPokemonCard ActivePokemon { get; }
    }

    internal interface IPlayerLogic : IPlayer
    {
        new string Name { get; set; }
        new bool IsActive { get; set; }
        new IPokemonCard ActivePokemon { get; set; }
        new IDeckLogic Deck { get; set; }
        IDeck IPlayer.Deck => Deck;
        new IHandLogic Hand { get; }
        ICardList IPlayer.Hand => Hand;
        new IBenchLogic Bench { get; }
        IBench IPlayer.Bench => Bench;
        new IPrizesLogic Prizes { get; }
        IPrizes IPlayer.Prizes => Prizes;

        new IDiscardPileLogic DiscardPile { get; }
        IDiscardPile IPlayer.DiscardPile => DiscardPile;
        void Draw(int amount);
        void SetPrizeCards();
    }

    internal class Player : IPlayerLogic
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public IPokemonCard ActivePokemon { get; set; }
        public IDeckLogic Deck { get; set; }
        public IHandLogic Hand { get; } = new Hand();
        public IBenchLogic Bench { get; } = new Bench();
        public IDiscardPileLogic DiscardPile { get; } = new DiscardPile();
        public IPrizesLogic Prizes { get; } = new Prizes();

        public void Draw(int amount)
        {
            var drawnCards = Deck.Draw(amount);
            if (drawnCards != null)
            {
                Hand.AddCards(drawnCards);
            }
        }

        public void SetPrizeCards()
        {
            Debug.Log($"Setting prizes for {Name}");
            var prizeCards = Deck.DrawFaceDown(6);
            Prizes.AddCards(prizeCards);
        }
    }
}
