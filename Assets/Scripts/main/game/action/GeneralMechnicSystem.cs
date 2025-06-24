using System;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game;
using gamecore.game.action;
using gamecore.gamegame.action;
using UnityEngine;

namespace gamecore.action
{
    class GeneralMechnicSystem
        : IActionPerformer<AttackGA>,
            IActionPerformer<DrawPrizeCardsGA>,
            IActionPerformer<CheckWinConditionGA>,
            IActionPerformer<PromoteGA>,
            IActionPerformer<RetreatGA>,
            IActionPerformer<PerformAbilityGA>,
            IActionPerformer<EvolveGA>
    {
        private static readonly Lazy<GeneralMechnicSystem> lazy = new(
            () => new GeneralMechnicSystem()
        );
        public static GeneralMechnicSystem INSTANCE => lazy.Value;

        private GeneralMechnicSystem() { }

        private readonly ActionSystem _actionSystem = ActionSystem.INSTANCE;
        private Game _game;

        public void Enable(Game game)
        {
            _actionSystem.AttachPerformer<AttackGA>(INSTANCE);
            _actionSystem.AttachPerformer<DrawPrizeCardsGA>(INSTANCE);
            _actionSystem.AttachPerformer<CheckWinConditionGA>(INSTANCE);
            _actionSystem.AttachPerformer<PromoteGA>(INSTANCE);
            _actionSystem.AttachPerformer<RetreatGA>(INSTANCE);
            _actionSystem.AttachPerformer<PerformAbilityGA>(INSTANCE);
            _actionSystem.AttachPerformer<EvolveGA>(INSTANCE);
            _game = game;
        }

        public void Disable()
        {
            _actionSystem.DetachPerformer<AttackGA>();
            _actionSystem.DetachPerformer<DrawPrizeCardsGA>();
            _actionSystem.DetachPerformer<CheckWinConditionGA>();
            _actionSystem.DetachPerformer<PromoteGA>();
            _actionSystem.DetachPerformer<RetreatGA>();
            _actionSystem.DetachPerformer<PerformAbilityGA>();
            _actionSystem.DetachPerformer<EvolveGA>();
        }

        public Task<AttackGA> Perform(AttackGA action)
        {
            foreach (var effect in action.Attack.Effects)
            {
                effect.Perform(action.Attacker);
            }
            return Task.FromResult(action);
        }

        public Task<DrawPrizeCardsGA> Perform(DrawPrizeCardsGA action)
        {
            foreach (var playerEntry in action.NumberOfPrizeCardsPerPlayer)
            {
                var prizes = playerEntry.Key.Prizes.TakePrizes(playerEntry.Value);
                action.DrawnCards.Add(playerEntry.Key.Name, prizes);
                playerEntry.Key.Hand.AddCards(prizes);
            }
            return Task.FromResult(action);
        }

        public Task<CheckWinConditionGA> Perform(CheckWinConditionGA action)
        {
            var numberOfWinConditionsPlayer1 = GetNumberOfWinConditionsForPlayer(action.Players[0]);
            var numberOfWinConditionsPlayer2 = GetNumberOfWinConditionsForPlayer(action.Players[1]);
            if (numberOfWinConditionsPlayer1 > 0 || numberOfWinConditionsPlayer2 > 0)
            {
                if (numberOfWinConditionsPlayer1 > numberOfWinConditionsPlayer2)
                    _game.EndGame(action.Players[0]);
                else if (numberOfWinConditionsPlayer2 > numberOfWinConditionsPlayer1)
                    _game.EndGame(action.Players[1]);
                else
                    _game.EndGame(null);
            }
            return Task.FromResult(action);
        }

        private static int GetNumberOfWinConditionsForPlayer(IPlayerLogic player)
        {
            int numberOfWinConditions = 0;
            if (player.Prizes.CardCount == 0)
                numberOfWinConditions++;
            if (player.Opponent.ActivePokemon == null && player.Opponent.Bench.CardCount == 0)
                numberOfWinConditions++;

            return numberOfWinConditions;
        }

        public async Task<PromoteGA> Perform(PromoteGA action)
        {
            foreach (var player in action.Players)
            {
                if (player.ActivePokemon == null)
                {
                    if (player.Bench.CardCount == 1)
                    {
                        player.Promote(player.Bench.Cards[0] as IPokemonCardLogic);
                    }
                    else
                    {
                        var selection = await _game.AwaitSelection(
                            player,
                            player.Bench.Cards,
                            1,
                            SelectFrom.InPlay
                        );
                        player.Promote(selection[0] as IPokemonCardLogic);
                    }
                }
            }
            return action;
        }

        public Task<RetreatGA> Perform(RetreatGA action)
        {
            var pokemon = action.Pokemon;
            _actionSystem.AddReaction(
                new DiscardAttachedEnergyCardsGA(pokemon, action.EnergyCardsToDiscard)
            );
            pokemon.Owner.ActivePokemon = null;
            _actionSystem.AddReaction(new PromoteGA(new() { pokemon.Owner }));
            _actionSystem.AddReaction(new MovePokemonToBenchGA(pokemon));
            pokemon.Owner.PerformedOncePerTurnActions.Add(PokemonCard.RETREATED);
            return Task.FromResult(action);
        }

        public Task<PerformAbilityGA> Perform(PerformAbilityGA action)
        {
            foreach (var effect in action.Pokemon.Ability.Effects)
            {
                effect.Perform(action.Pokemon);
            }
            action.Pokemon.AbilityUsedThisTurn = true;
            ActionSystem.INSTANCE.SubscribeToGameAction<EndTurnGA>(
                action.Pokemon,
                ReactionTiming.PRE
            );
            return Task.FromResult(action);
        }

        public Task<EvolveGA> Perform(EvolveGA action)
        {
            EvolvePokemon(action.NewPokemon, action.TargetPokemon);
            return Task.FromResult(action);
        }

        public Task<EvolveGA> Reperform(EvolveGA action)
        {
            var newPokemon = _game.FindCardAnywhere(action.NewPokemon) as IPokemonCardLogic;
            var targetPokemon = _game.FindCardAnywhere(action.TargetPokemon) as IPokemonCardLogic;
            EvolvePokemon(newPokemon, targetPokemon);
            return Task.FromResult(action);
        }

        private static void EvolvePokemon(
            IPokemonCardLogic newPokemon,
            IPokemonCardLogic targetPokemon
        )
        {
            newPokemon.Owner.Hand.RemoveCard(newPokemon);
            newPokemon.PreEvolutions.Add(targetPokemon);

            if (targetPokemon == targetPokemon.Owner.ActivePokemon)
                targetPokemon.Owner.ActivePokemon = newPokemon;
            else
            {
                targetPokemon.Owner.Bench.ReplaceInPlace(targetPokemon, newPokemon);
            }

            newPokemon.AttachEnergyCards(targetPokemon.AttachedEnergyCards);
            targetPokemon.AttachedEnergyCards.Clear();
            newPokemon.TakeDamage(targetPokemon.Damage);

            foreach (var preEvolution in targetPokemon.PreEvolutions)
                newPokemon.PreEvolutions.Add(preEvolution);

            targetPokemon.PreEvolutions.Clear();

            targetPokemon.WasEvolved();
            targetPokemon.SetPutInPlay();
        }
    }
}
