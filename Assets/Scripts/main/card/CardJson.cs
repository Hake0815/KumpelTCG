using System;

namespace gamecore.card
{
    [Serializable]
    public class CardJson
    {
        public string Name { get; }
        public string CardType { get; }
        public string CardSubtype { get; }
        public EnergyType EnergyType { get; }
        public int Hp { get; }
    }
}
