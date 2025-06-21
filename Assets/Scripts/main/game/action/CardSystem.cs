using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using UnityEngine;

namespace gamecore.game.action
{
    class CardSystem
        : IActionPerformer<DrawCardGA>,
            IActionPerformer<DiscardCardsFromHandGA>,
            IActionPerformer<AttachEnergyFromHandGA>,
            IActionPerformer<AttachEnergyFromHandForTurnGA>,
            IActionPerformer<DiscardAttachedEnergyCardsGA>,
            IActionPerformer<BenchPokemonGA>,
            IActionPerformer<MovePokemonToBenchGA>,
            IActionPerformer<ResetPokemonTurnStateGA>,
            IActionPerformer<RevealCardsFromDeckGA>,
            IActionPerformer<TakeSelectionToHandGA>,
            IActionPerformer<PutRemainingCardsUnderDeckGA>,
            IActionPerformer<PlayCardGA>,
            IActionPerformer<SetActivePokemonGA>,
            IActionSubscriber<StartTurnGA>
    {
        private static readonly Lazy<CardSystem> lazy = new(() => new CardSystem());
        public static CardSystem INSTANCE => lazy.Value;

        private CardSystem() { }

        protected static readonly System.Random _rng = new();

        private readonly ActionSystem _actionSystem = ActionSystem.INSTANCE;
        private Game _game;

        public void Enable(Game game)
        {
            _actionSystem.AttachPerformer<DrawCardGA>(INSTANCE);
            _actionSystem.AttachPerformer<DiscardCardsFromHandGA>(INSTANCE);
            _actionSystem.AttachPerformer<AttachEnergyFromHandGA>(INSTANCE);
            _actionSystem.AttachPerformer<AttachEnergyFromHandForTurnGA>(INSTANCE);
            _actionSystem.AttachPerformer<DiscardAttachedEnergyCardsGA>(INSTANCE);
            _actionSystem.AttachPerformer<BenchPokemonGA>(INSTANCE);
            _actionSystem.AttachPerformer<MovePokemonToBenchGA>(INSTANCE);
            _actionSystem.AttachPerformer<ResetPokemonTurnStateGA>(INSTANCE);
            _actionSystem.AttachPerformer<RevealCardsFromDeckGA>(INSTANCE);
            _actionSystem.AttachPerformer<TakeSelectionToHandGA>(INSTANCE);
            _actionSystem.AttachPerformer<PutRemainingCardsUnderDeckGA>(INSTANCE);
            _actionSystem.AttachPerformer<PlayCardGA>(INSTANCE);
            _actionSystem.AttachPerformer<SetActivePokemonGA>(INSTANCE);
            _actionSystem.SubscribeToGameAction<StartTurnGA>(INSTANCE, ReactionTiming.POST);
            _game = game;
        }

        public void Disable()
        {
            _actionSystem.DetachPerformer<DrawCardGA>();
            _actionSystem.DetachPerformer<DiscardCardsFromHandGA>();
            _actionSystem.DetachPerformer<AttachEnergyFromHandGA>();
            _actionSystem.DetachPerformer<AttachEnergyFromHandForTurnGA>();
            _actionSystem.DetachPerformer<DiscardAttachedEnergyCardsGA>();
            _actionSystem.DetachPerformer<BenchPokemonGA>();
            _actionSystem.DetachPerformer<ResetPokemonTurnStateGA>();
            _actionSystem.DetachPerformer<RevealCardsFromDeckGA>();
            _actionSystem.DetachPerformer<TakeSelectionToHandGA>();
            _actionSystem.DetachPerformer<PutRemainingCardsUnderDeckGA>();
            _actionSystem.DetachPerformer<PlayCardGA>();
            _actionSystem.DetachPerformer<SetActivePokemonGA>();
            _actionSystem.UnsubscribeFromGameAction<StartTurnGA>(INSTANCE, ReactionTiming.POST);
        }

        public StartTurnGA React(StartTurnGA startTurnGA)
        {
            var drawCardGA = new DrawCardGA(1, startTurnGA.NextPlayer);
            _actionSystem.AddReaction(drawCardGA);
            return startTurnGA;
        }

        public Task<DrawCardGA> Perform(DrawCardGA drawCardGA)
        {
            var drawnCards = drawCardGA.Player.Draw(drawCardGA.Amount);
            drawCardGA.DrawnCards.AddRange(drawnCards);
            return Task.FromResult(drawCardGA);
        }

        public Task<DiscardCardsFromHandGA> Perform(DiscardCardsFromHandGA action)
        {
            foreach (var card in action.Cards)
            {
                card.Discard();
                card.Owner.Hand.RemoveCards(new() { card });
            }
            return Task.FromResult(action);
        }

        public Task<AttachEnergyFromHandGA> Perform(AttachEnergyFromHandGA action)
        {
            AttachEnergyFromHand(action);
            return Task.FromResult(action);
        }

        public Task<AttachEnergyFromHandForTurnGA> Perform(AttachEnergyFromHandForTurnGA action)
        {
            AttachEnergyFromHand(action);
            action.EnergyCard.Owner.PerformedOncePerTurnActions.Add(
                EnergyCard.ATTACHED_ENERGY_FOR_TURN
            );
            return Task.FromResult(action);
        }

        private static void AttachEnergyFromHand(AttachEnergyFromHandGA action)
        {
            var energyCard = action.EnergyCard;
            energyCard.Owner.Hand.RemoveCard(energyCard);
            action.TargetPokemon.AttachEnergyCards(new() { energyCard });
        }

        public Task<DiscardAttachedEnergyCardsGA> Perform(DiscardAttachedEnergyCardsGA action)
        {
            action.Pokemon.DiscardEnergy(action.EnergyCards);
            return Task.FromResult(action);
        }

        public Task<BenchPokemonGA> Perform(BenchPokemonGA action)
        {
            var pokemon = action.Card;
            pokemon.Owner.Bench.AddCards(new() { pokemon });
            pokemon.Owner.Hand.RemoveCard(pokemon);
            pokemon.SetPutInPlay();
            return Task.FromResult(action);
        }

        public Task<MovePokemonToBenchGA> Perform(MovePokemonToBenchGA action)
        {
            var pokemon = action.Pokemon;
            pokemon.Owner.Bench.AddCards(new() { pokemon });
            return Task.FromResult(action);
        }

        public Task<ResetPokemonTurnStateGA> Perform(ResetPokemonTurnStateGA action)
        {
            action.PokemonToReset.PutIntoPlayThisTurn = false;
            action.PokemonToReset.AbilityUsedThisTurn = false;
            ActionSystem.INSTANCE.UnsubscribeFromGameAction<EndTurnGA>(
                action.PokemonToReset,
                ReactionTiming.PRE
            );
            return Task.FromResult(action);
        }

        public Task<RevealCardsFromDeckGA> Perform(RevealCardsFromDeckGA action)
        {
            var revealedCards = action.Player.Deck.Draw(action.Count);
            action.RevealedCards.AddRange(revealedCards);
            return Task.FromResult(action);
        }

        public async Task<TakeSelectionToHandGA> Perform(TakeSelectionToHandGA action)
        {
            var selectedCards = await _game.AwaitSelection(
                action.Player,
                action.Options,
                action.Amount,
                SelectFrom.Floating
            );
            action.Player.Hand.AddCards(selectedCards);
            action.RemainingCards.AddRange(action.Options.Except(selectedCards));
            return action;
        }

        public Task<PutRemainingCardsUnderDeckGA> Perform(PutRemainingCardsUnderDeckGA action)
        {
            action.Player.Deck.AddCards(Shuffle(action.RemainingCards));
            return Task.FromResult(action);
        }

        private static List<ICardLogic> Shuffle(List<ICardLogic> cards)
        {
            var n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = _rng.Next(n + 1);
                (cards[n], cards[k]) = (cards[k], cards[n]);
            }
            return cards;
        }

        public Task<PlayCardGA> Perform(PlayCardGA action)
        {
            if (action.Targets != null)
                action.Card.PlayWithTargets(action.Targets);
            else
                action.Card.Play();
            return Task.FromResult(action);
        }

        public Task<PlayCardGA> Reperform(PlayCardGA action)
        {
            var card = _game
                .GetPlayerByName(action.Card.Owner.Name)
                .Hand.GetCardByDeckId(action.Card.DeckId);
            if (action.Targets != null)
            {
                var targets = _game.FindCardsAnywhere(action.Targets);
                card.PlayWithTargets(targets);
            }
            else
                card.Play();
            return Task.FromResult(action);
        }

        public Task<SetActivePokemonGA> Perform(SetActivePokemonGA action)
        {
            action.Card.Play();
            return Task.FromResult(action);
        }

        public Task<SetActivePokemonGA> Reperform(SetActivePokemonGA action)
        {
            Debug.Log("Replaying SetActivePokemonGA");
            var card = _game
                .GetPlayerByName(action.Card.Owner.Name)
                .Hand.GetCardByDeckId(action.Card.DeckId);
            card.Play();
            _game.AdvanceGameStateQuietly();
            return Task.FromResult(action);
        }
    }
}
