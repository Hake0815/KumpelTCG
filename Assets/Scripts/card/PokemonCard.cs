using System;
using gamecore.game;

namespace gamecore.card
{
    public interface IPokemonCard : ICard
    {
        Stage Stage { get; }
    }

    internal class PokemonCard : IPokemonCard, ICardLogic
    {
        public ICardData CardData { get; }
        public IPokemonCardData PokemonCardData
        {
            get => CardData as IPokemonCardData;
        }
        public IPlayerLogic Owner { get; }
        public Stage Stage => PokemonCardData.Stage;

        IPlayer ICard.Owner => Owner;

        public event Action CardDiscarded;

        public PokemonCard(IPokemonCardData cardData, IPlayerLogic owner)
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
            return Owner.Hand.Cards.Contains(this) && Owner.IsActive && Stage == Stage.Basic;
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
            if (Owner.ActivePokemon == null)
            {
                Owner.ActivePokemon = this;
                Owner.Hand.RemoveCard(this);
            }
        }
    }
}
