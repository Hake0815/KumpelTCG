using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.common;
using gamecore.effect;
using gamecore.game.interaction;

namespace gamecore.game.action
{
    class GeneralMechnicSystem
        : IActionPerformer<AttackGA>,
            IActionPerformer<DrawPrizeCardsGA>,
            IActionPerformer<CheckWinConditionGA>,
            IActionPerformer<PromoteGA>,
            IActionPerformer<RetreatGA>,
            IActionPerformer<PerformAbilityGA>,
            IActionPerformer<PlaySupporterGA>,
            IActionPerformer<RemovePlayerEffectGA>,
            IActionPerformer<RemovePokemonEffectGA>,
            IActionPerformer<EvolveGA>
    {
        public GeneralMechnicSystem(ActionSystem actionSystem, Game game)
        {
            _actionSystem = actionSystem;
            _game = game;
            Enable();
        }

        private readonly ActionSystem _actionSystem;
        private readonly Game _game;

        public void Enable()
        {
            _actionSystem.AttachPerformer<AttackGA>(this);
            _actionSystem.AttachPerformer<DrawPrizeCardsGA>(this);
            _actionSystem.AttachPerformer<CheckWinConditionGA>(this);
            _actionSystem.AttachPerformer<PromoteGA>(this);
            _actionSystem.AttachPerformer<RetreatGA>(this);
            _actionSystem.AttachPerformer<PerformAbilityGA>(this);
            _actionSystem.AttachPerformer<PlaySupporterGA>(this);
            _actionSystem.AttachPerformer<EvolveGA>(this);
            _actionSystem.AttachPerformer<RemovePlayerEffectGA>(this);
            _actionSystem.AttachPerformer<RemovePokemonEffectGA>(this);
        }

        public void Disable()
        {
            _actionSystem.DetachPerformer<AttackGA>();
            _actionSystem.DetachPerformer<DrawPrizeCardsGA>();
            _actionSystem.DetachPerformer<CheckWinConditionGA>();
            _actionSystem.DetachPerformer<PromoteGA>();
            _actionSystem.DetachPerformer<RetreatGA>();
            _actionSystem.DetachPerformer<PerformAbilityGA>();
            _actionSystem.DetachPerformer<PlaySupporterGA>();
            _actionSystem.DetachPerformer<EvolveGA>();
            _actionSystem.DetachPerformer<RemovePlayerEffectGA>();
            _actionSystem.DetachPerformer<RemovePokemonEffectGA>();
        }

        public Task<AttackGA> Perform(AttackGA action)
        {
            foreach (var instruction in action.Attack.Instructions)
            {
                instruction.Perform(action.Attacker, _actionSystem);
            }
            return Task.FromResult(action);
        }

        public Task<AttackGA> Reperform(AttackGA action)
        {
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

        public Task<DrawPrizeCardsGA> Reperform(DrawPrizeCardsGA action)
        {
            foreach (var playerEntry in action.DrawnCards)
            {
                var player = _game.GetPlayerByName(playerEntry.Key);
                var prizes = player.DeckList.GetCardsByDeckIds(playerEntry.Value);
                player.Prizes.RemoveCards(prizes);
                player.Hand.AddCards(prizes);
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
                {
                    action.GameEnded = true;
                    action.Winner = action.Players[0];
                    _game.EndGame(action.Players[0], GetWinMessage(action.Players[0]));
                }
                else if (numberOfWinConditionsPlayer2 > numberOfWinConditionsPlayer1)
                {
                    action.GameEnded = true;
                    action.Winner = action.Players[1];
                    _game.EndGame(action.Players[1], GetWinMessage(action.Players[1]));
                }
                else
                {
                    action.GameEnded = true;
                    _game.EndGame(null, "Game ended in a draw!");
                }
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

        private static string GetWinMessage(IPlayerLogic winner)
        {
            var messageBuilder = new StringBuilder(string.Format("Winner is {0}!", winner.Name));
            if (winner.Prizes.CardCount == 0)
                messageBuilder.AppendLine(
                    string.Format("Winner has {0} prize cards!", winner.Prizes.CardCount)
                );
            if (winner.Opponent.ActivePokemon == null && winner.Opponent.Bench.CardCount == 0)
                messageBuilder.AppendLine("Opponent has no Pokemon left!");

            return messageBuilder.ToString();
        }

        public Task<CheckWinConditionGA> Reperform(CheckWinConditionGA action)
        {
            if (action.GameEnded)
            {
                action.Winner.Let(
                    winnerStub =>
                        _game.EndGame(
                            _game.GetPlayerByName(winnerStub.Name),
                            GetWinMessage(_game.GetPlayerByName(winnerStub.Name))
                        ),
                    () => _game.EndGame(null, "Game ended in a draw!")
                );
            }
            return Task.FromResult(action);
        }

        public async Task<PromoteGA> Perform(PromoteGA action)
        {
            foreach (var player in action.Players)
            {
                if (player.ActivePokemon == null)
                {
                    var pokemon =
                        player.Bench.CardCount == 1
                            ? player.Bench.Cards[0]
                            : await GetPokemonFromUserSelection(player);
                    player.Promote(pokemon as IPokemonCardLogic);
                    action.PromotedPokemon.Add(player.Name, pokemon as IPokemonCardLogic);
                }
            }
            return action;
        }

        private async Task<IPokemonCardLogic> GetPokemonFromUserSelection(IPlayerLogic player)
        {
            var selection = await _game.AwaitSelection(
                player,
                player.Bench.Cards.ToList(),
                new ConditionalTargetQuery(new NumberRange(1, 1), SelectionQualifier.NumberOfCards),
                true,
                SelectFrom.InPlay,
                ActionOnSelection.Promote,
                ActionOnSelection.Nothing
            );
            return selection[0] as IPokemonCardLogic;
        }

        public Task<PromoteGA> Reperform(PromoteGA action)
        {
            foreach (var playerPokemonEntry in action.PromotedPokemon)
            {
                var player = _game.GetPlayerByName(playerPokemonEntry.Key);
                var pokemon = player.DeckList.GetCardByDeckId(playerPokemonEntry.Value.DeckId);
                player.Promote(pokemon as IPokemonCardLogic);
            }
            return Task.FromResult(action);
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

        public Task<RetreatGA> Reperform(RetreatGA action)
        {
            var pokemon = _game.FindCardAnywhere(action.Pokemon) as IPokemonCardLogic;
            pokemon.Owner.ActivePokemon = null;
            pokemon.Owner.PerformedOncePerTurnActions.Add(PokemonCard.RETREATED);
            return Task.FromResult(action);
        }

        public Task<PerformAbilityGA> Perform(PerformAbilityGA action)
        {
            foreach (var instruction in action.Pokemon.Ability.Instructions)
            {
                instruction.Perform(action.Pokemon, _actionSystem);
            }
            (
                (PokemonEffectAbstract)new AbilityUsedThisTurnEffect(_actionSystem, action.Pokemon)
            ).Apply();
            return Task.FromResult(action);
        }

        public Task<PerformAbilityGA> Reperform(PerformAbilityGA action)
        {
            var pokemon = (IPokemonCardLogic)_game.FindCardAnywhere(action.Pokemon);
            ((PokemonEffectAbstract)new AbilityUsedThisTurnEffect(_actionSystem, pokemon)).Apply();
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

        private void EvolvePokemon(IPokemonCardLogic newPokemon, IPokemonCardLogic targetPokemon)
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
            targetPokemon.SetPutInPlay(_actionSystem);
        }

        public Task<PlaySupporterGA> Perform(PlaySupporterGA action)
        {
            action.Player.PerformedOncePerTurnActions.Add(SupporterCard.PLAYED_SUPPORTER_THIS_TURN);
            return Task.FromResult(action);
        }

        public Task<PlaySupporterGA> Reperform(PlaySupporterGA action)
        {
            _game
                .GetPlayerByName(action.Player.Name)
                .PerformedOncePerTurnActions.Add(SupporterCard.PLAYED_SUPPORTER_THIS_TURN);
            return Task.FromResult(action);
        }

        public Task<RemovePlayerEffectGA> Perform(RemovePlayerEffectGA action)
        {
            RemoveEffectFromPlayer(action.Player, action.Effect);
            return Task.FromResult(action);
        }

        public Task<RemovePlayerEffectGA> Reperform(RemovePlayerEffectGA action)
        {
            var player = _game.GetPlayerByName(action.Player.Name);
            RemoveEffectFromPlayer(player, action.Effect);

            return Task.FromResult(action);
        }

        private void RemoveEffectFromPlayer(IPlayerLogic player, PlayerEffectAbstract effect)
        {
            player.RemoveEffect(effect);
            if (effect is FirstTurnOfGameEffect firstTurnOfGameEffect)
            {
                _actionSystem.UnsubscribeFromGameAction<EndTurnGA>(
                    firstTurnOfGameEffect,
                    ReactionTiming.POST
                );
            }
        }

        public Task<RemovePokemonEffectGA> Perform(RemovePokemonEffectGA action)
        {
            action.Pokemon.RemoveEffect(action.Effect);
            return Task.FromResult(action);
        }

        public Task<RemovePokemonEffectGA> Reperform(RemovePokemonEffectGA action)
        {
            var pokemon = _game.FindCardAnywhere(action.Pokemon) as IPokemonCardLogic;
            pokemon.RemoveEffect(action.Effect);
            return Task.FromResult(action);
        }
    }
}
