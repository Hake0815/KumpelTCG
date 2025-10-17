using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game;
using gamecore.game.action;
using gamecore.gamegame.action;

namespace gamecore.action
{
    class DamageSystem
        : IActionPerformer<DealDamgeGA>,
            IActionPerformer<KnockOutCheckGA>,
            IActionPerformer<KnockOutGA>
    {
        public DamageSystem(ActionSystem actionSystem, Game game)
        {
            _actionSystem = actionSystem;
            _game = game;
            Enable();
        }

        private readonly ActionSystem _actionSystem;
        private readonly Game _game;

        public void Enable()
        {
            _actionSystem.AttachPerformer<DealDamgeGA>(this);
            _actionSystem.AttachPerformer<KnockOutCheckGA>(this);
            _actionSystem.AttachPerformer<KnockOutGA>(this);
        }

        public void Disable()
        {
            _actionSystem.DetachPerformer<DealDamgeGA>();
            _actionSystem.DetachPerformer<KnockOutCheckGA>();
            _actionSystem.DetachPerformer<KnockOutGA>();
        }

        public Task<DealDamgeGA> Perform(DealDamgeGA action)
        {
            action.Damage += action.ModifierBeforeWeaknessResistance;
            if (action.Target.IsActive())
                ApplyWeaknessResistance(action);
            action.Damage += action.ModifierAfterWeaknessResistance;
            action.Damage = Math.Max(0, action.Damage);
            action.Target.TakeDamage(action.Damage);
            return Task.FromResult(action);
        }

        private static void ApplyWeaknessResistance(DealDamgeGA action)
        {
            if (action.Target.Weakness == action.Attacker.PokemonType)
                action.Damage *= 2;
            else if (action.Target.Resistance == action.Attacker.PokemonType)
                action.Damage -= 30;
        }

        public Task<DealDamgeGA> Reperform(DealDamgeGA action)
        {
            var target = _game.FindCardAnywhere(action.Target) as IPokemonCardLogic;
            target.TakeDamage(action.Damage);
            return Task.FromResult(action);
        }

        public Task<KnockOutCheckGA> Perform(KnockOutCheckGA action)
        {
            var numberOfPrizeCardsPerPlayer = new Dictionary<IPlayerLogic, int>();
            foreach (var player in action.Players)
            {
                AddPokemonIfKnockedOut(player.ActivePokemon, numberOfPrizeCardsPerPlayer);
                foreach (var card in player.Bench.Cards)
                {
                    AddPokemonIfKnockedOut(card as IPokemonCardLogic, numberOfPrizeCardsPerPlayer);
                }
            }

            if (numberOfPrizeCardsPerPlayer.Count > 0)
            {
                _actionSystem.AddReaction(new DrawPrizeCardsGA(numberOfPrizeCardsPerPlayer));
                _actionSystem.AddReaction(new CheckWinConditionGA(action.Players));
                _actionSystem.AddReaction(new PromoteGA(action.Players));
            }

            return Task.FromResult(action);
        }

        void AddPokemonIfKnockedOut(
            IPokemonCardLogic pokemon,
            Dictionary<IPlayerLogic, int> numberOfPrizeCardsPerPlayer
        )
        {
            if (pokemon.IsKnockedOut())
            {
                _actionSystem.AddReaction(new KnockOutGA(pokemon));
                if (!numberOfPrizeCardsPerPlayer.ContainsKey(pokemon.Owner.Opponent))
                    numberOfPrizeCardsPerPlayer[pokemon.Owner.Opponent] = 0;
                numberOfPrizeCardsPerPlayer[pokemon.Owner.Opponent] +=
                    pokemon.NumberOfPrizeCardsOnKnockout;
            }
        }

        public Task<KnockOutCheckGA> Reperform(KnockOutCheckGA action)
        {
            return Task.FromResult(action);
        }

        public Task<KnockOutGA> Perform(KnockOutGA action)
        {
            var pokemon = action.Pokemon;
            KnockOutPokemon(pokemon);
            return Task.FromResult(action);
        }

        public Task<KnockOutGA> Reperform(KnockOutGA action)
        {
            var pokemon = _game.FindCardAnywhere(action.Pokemon) as IPokemonCardLogic;
            KnockOutPokemon(pokemon);
            return Task.FromResult(action);
        }

        private static void KnockOutPokemon(IPokemonCardLogic pokemon)
        {
            pokemon.Discard();
            RemovePokemonFromPlay(pokemon);
            foreach (var energy in pokemon.AttachedEnergyCards)
            {
                energy.Discard();
            }
        }

        private static void RemovePokemonFromPlay(IPokemonCardLogic pokemon)
        {
            if (pokemon.Owner.ActivePokemon == pokemon)
                pokemon.Owner.ActivePokemon = null;
            else
                pokemon.Owner.Bench.RemoveCard(pokemon);
        }
    }
}
