using System;
using gamecore.card;

namespace gamecore.instruction.filter
{
    static class FilterUtils
    {
        public static FilterNode CreatePokemonFilter()
        {
            return new FilterCondition(
                FilterType.CardType,
                FilterOperation.Equals,
                CardType.Pokemon
            );
        }

        public static FilterNode CreatePokemonOrBasicEnergyFilter()
        {
            return new OrNode(new() { CreatePokemonFilter(), CreateBasicEnergyFilter() });
        }

        public static FilterNode CreateBasicEnergyFilter()
        {
            return new FilterCondition(
                FilterType.CardSubtype,
                FilterOperation.Equals,
                CardSubtype.BasicEnergy
            );
        }
    }
}
