using System;
using gamecore.serialization;

namespace gamecore.game
{
    public enum PokemonTurnTrait
    {
        PutInPlayThisTurn,
        AbilityUsedThisTurn,
    }

    public static class ToProtoBufPokemonTurnTraitExtensions
    {
        public static ProtoBufPokemonTurnTrait ToProtoBuf(this PokemonTurnTrait pokemonTurnTrait)
        {
            return pokemonTurnTrait switch
            {
                PokemonTurnTrait.PutInPlayThisTurn =>
                    ProtoBufPokemonTurnTrait.PokemonTurnTraitPutInPlayThisTurn,
                PokemonTurnTrait.AbilityUsedThisTurn =>
                    ProtoBufPokemonTurnTrait.PokemonTurnTraitAbilityUsedThisTurn,
                _ => throw new InvalidOperationException(
                    $"Invalid pokemon turn trait: {pokemonTurnTrait}"
                ),
            };
        }
    }
}
