using System;
using gamecore.game;

namespace gamecore.card
{
    public interface IPokemonCard : ICard
    {
        public Stage Stage { get; }
    }

    internal class PokemonCard : IPokemonCard, ICardLogic
    {
        private IPlayerLogic _owner;
        public ICardData CardData { get; }
        public IPokemonCardData PokemonCardData
        {
            get => CardData as IPokemonCardData;
        }
        IPlayerLogic ICardLogic.Owner
        {
            get => _owner;
        }
        public Stage Stage => PokemonCardData.Stage;
        public event Action CardDiscarded;

        internal PokemonCard(IPokemonCardData cardData, IPlayerLogic owner)
        {
            CardData = cardData;
            _owner = owner;
        }

        void ICardLogic.Discard()
        {
            _owner.DiscardPile.AddCards(new() { this });
            CardDiscarded?.Invoke();
        }

        bool ICardLogic.IsPlayable()
        {
            if (!_owner.Hand.Cards.Contains(this))
                return false;

            if (_owner.ActivePokemon == null)
                return Stage == Stage.Basic;

            return _owner.IsActive && Stage == Stage.Basic;
        }

        bool ICardLogic.IsPokemonCard()
        {
            return true;
        }

        bool ICardLogic.IsTrainerCard()
        {
            return false;
        }

        void ICardLogic.Play()
        {
            if (_owner.ActivePokemon == null)
            {
                _owner.ActivePokemon = this;
                _owner.Hand.RemoveCard(this);
            }
        }
    }
}
