using System;
using gamecore.actionsystem;
using gamecore.game;
using gamecore.game.action;

namespace gamecore.card
{
    public interface IPokemonCard : ICard
    {
        Stage Stage { get; }
    }

    internal class PokemonCard : IPokemonCard, ICardLogic
    {
        public ICardData CardData => PokemonCardData;

        public IPokemonCardData PokemonCardData { get; }
        public IPlayerLogic Owner { get; }
        public Stage Stage => PokemonCardData.Stage;
        public event Action CardDiscarded;

        public PokemonCard(IPokemonCardData cardData, IPlayerLogic owner)
        {
            PokemonCardData = cardData;
            Owner = owner;
        }

        public void Discard()
        {
            Owner.DiscardPile.AddCards(new() { this });
            CardDiscarded?.Invoke();
        }

        public bool IsPlayable()
        {
            return Owner.Hand.Cards.Contains(this)
                && Owner.IsActive
                && Stage == Stage.Basic
                && !Owner.Bench.Full;
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
                return;
            }
            if (!Owner.Bench.Full)
            {
                ActionSystem.INSTANCE.Perform(new BenchPokemonGA(this));
            }
        }
    }
}
