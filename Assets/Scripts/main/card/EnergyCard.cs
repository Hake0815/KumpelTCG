using System;
using gamecore.game;

namespace gamecore.card
{
    public interface IEnergyCard : ICard { }

    internal interface IEnergyCardLogic : IEnergyCard, ICardLogic { }

    internal class EnergyCard : IEnergyCardLogic
    {
        public IPlayerLogic Owner { get; }

        public ICardData CardData => throw new NotImplementedException();

        IPlayer ICard.Owner => Owner;

        public event Action CardDiscarded;

        public void Discard()
        {
            throw new NotImplementedException();
        }

        public bool IsPlayable()
        {
            throw new NotImplementedException();
        }

        public bool IsPokemonCard()
        {
            throw new NotImplementedException();
        }

        public bool IsTrainerCard()
        {
            throw new NotImplementedException();
        }

        public void Play()
        {
            throw new NotImplementedException();
        }
    }
}
