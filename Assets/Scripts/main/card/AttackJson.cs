using System;
using System.Collections.Generic;

namespace gamecore.card
{
    [Serializable]
    public class AttackJson
    {
        public int Damage { get; }
        public List<EnergyType> EnergyCost;
    }
}
