using System;
using gamecore.game;

namespace gamecore.card
{
    public interface IPokemonCard : ICard
    {
        public Stage Stage { get; }
    }

    public class PokemonCard : IPokemonCard
    {
        public ICardData CardData { get; }
        public IPokemonCardData PokemonCardData
        {
            get => CardData as IPokemonCardData;
        }
        public IPlayer Owner { get; }
        public Stage Stage => PokemonCardData.Stage;
        public event Action CardDiscarded;

        public PokemonCard(IPokemonCardData cardData, IPlayer owner)
        {
            CardData = cardData;
            Owner = owner;
        }

        public void Discard()
        {
            Owner.DiscardPile.AddCards(new() { this });
            CardDiscarded?.Invoke();
        }

        public bool IsPlayable()
        {
            return false;
        }

        public bool IsPokemonCard()
        {
            return true;
        }

        public bool IsTrainerCard()
        {
            return false;
        }

        public void Play()
        {
            // not playable yet
        }
    }
}
