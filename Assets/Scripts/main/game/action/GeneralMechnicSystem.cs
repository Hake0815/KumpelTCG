using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game;
using gamecore.game.action;
using gamecore.gamegame.action;
using UnityEngine;

namespace gamecore.action
{
    internal class GeneralMechnicSystem
        : IActionPerformer<AttackGA>,
            IActionPerformer<DrawPrizeCardsGA>,
            IActionPerformer<CheckWinConditionGA>,
            IActionPerformer<PromoteGA>,
            IActionPerformer<RetreatGA>
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
            _game = game;
        }

        public void Disable()
        {
            _actionSystem.DetachPerformer<AttackGA>();
            _actionSystem.DetachPerformer<DrawPrizeCardsGA>();
            _actionSystem.DetachPerformer<CheckWinConditionGA>();
            _actionSystem.DetachPerformer<PromoteGA>();
            _actionSystem.DetachPerformer<RetreatGA>();
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
                        var selection = await _game.AwaitSelection(player, player.Bench.Cards, 1);
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
    }
}
