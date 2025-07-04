using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.state;
using UnityEngine;
using static gamecore.game.action.SelectCardsGA;

namespace gamecore.game.action
{
    class CardSystem
        : IActionPerformer<DrawCardGA>,
            IActionPerformer<DrawMulliganCardsGA>,
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
            IActionPerformer<SelectExactCardsGA>,
            IActionPerformer<SelectUpToCardsGA>,
            IActionPerformer<DiscardCardsGA>,
            IActionPerformer<RemoveCardFromHandGA>,
            IActionSubscriber<StartTurnGA>
    {
        private static readonly Lazy<CardSystem> lazy = new(() => new CardSystem());
        public static CardSystem INSTANCE => lazy.Value;

        private CardSystem() { }

        protected static readonly System.Random _rng = new();

        private readonly ActionSystem _actionSystem = ActionSystem.INSTANCE;
        private static readonly Dictionary<SelectedCardsOrigin, SelectFrom> _selectFromMap = new()
        {
            { SelectedCardsOrigin.Hand, SelectFrom.InPlay },
            { SelectedCardsOrigin.Other, SelectFrom.Floating },
            { SelectedCardsOrigin.Deck, SelectFrom.Deck },
            { SelectedCardsOrigin.DiscardPile, SelectFrom.DiscardPile },
        };

        private Game _game;

        public void Enable(Game game)
        {
            _actionSystem.AttachPerformer<DrawCardGA>(INSTANCE);
            _actionSystem.AttachPerformer<DrawMulliganCardsGA>(INSTANCE);
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
            _actionSystem.AttachPerformer<SelectExactCardsGA>(INSTANCE);
            _actionSystem.AttachPerformer<SelectUpToCardsGA>(INSTANCE);
            _actionSystem.AttachPerformer<DiscardCardsGA>(INSTANCE);
            _actionSystem.AttachPerformer<RemoveCardFromHandGA>(INSTANCE);
            _actionSystem.SubscribeToGameAction<StartTurnGA>(INSTANCE, ReactionTiming.POST);
            _game = game;
        }

        public void Disable()
        {
            _actionSystem.DetachPerformer<DrawCardGA>();
            _actionSystem.DetachPerformer<DrawMulliganCardsGA>();
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
            _actionSystem.DetachPerformer<SelectExactCardsGA>();
            _actionSystem.DetachPerformer<SelectUpToCardsGA>();
            _actionSystem.DetachPerformer<DiscardCardsGA>();
            _actionSystem.DetachPerformer<RemoveCardFromHandGA>();
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

        public Task<DrawCardGA> Reperform(DrawCardGA drawCardGA)
        {
            var player = _game.GetPlayerByName(drawCardGA.Player.Name);
            var drawnCards = player.DeckList.GetCardsByDeckIds(drawCardGA.DrawnCards);
            player.Deck.RemoveCards(drawnCards);
            player.Hand.AddCards(drawnCards);
            return Task.FromResult(drawCardGA);
        }

        public Task<DrawMulliganCardsGA> Perform(DrawMulliganCardsGA drawCardGA)
        {
            _actionSystem.AddReaction(new DrawCardGA(drawCardGA.Amount, drawCardGA.Player));
            return Task.FromResult(drawCardGA);
        }

        public Task<DrawMulliganCardsGA> Reperform(DrawMulliganCardsGA drawCardGA)
        {
            _game.GameState = new SelectBenchPokemonState();
            return Task.FromResult(drawCardGA);
        }

        public Task<AttachEnergyFromHandGA> Perform(AttachEnergyFromHandGA action)
        {
            AttachEnergyFromHand(action.EnergyCard, action.TargetPokemon);
            return Task.FromResult(action);
        }

        public Task<AttachEnergyFromHandGA> Reperform(AttachEnergyFromHandGA action)
        {
            var energyCard =
                _game
                    .GetPlayerByName(action.EnergyCard.Owner.Name)
                    .DeckList.GetCardByDeckId(action.EnergyCard.DeckId) as IEnergyCardLogic;
            var targetPokemon = _game.FindCardAnywhere(action.TargetPokemon) as IPokemonCardLogic;
            AttachEnergyFromHand(energyCard, targetPokemon);
            return Task.FromResult(action);
        }

        public Task<AttachEnergyFromHandForTurnGA> Perform(AttachEnergyFromHandForTurnGA action)
        {
            AttachEnergyFromHand(action.EnergyCard, action.TargetPokemon);
            action.EnergyCard.Owner.PerformedOncePerTurnActions.Add(
                EnergyCard.ATTACHED_ENERGY_FOR_TURN
            );
            return Task.FromResult(action);
        }

        public Task<AttachEnergyFromHandForTurnGA> Reperform(AttachEnergyFromHandForTurnGA action)
        {
            var energyCard =
                _game
                    .GetPlayerByName(action.EnergyCard.Owner.Name)
                    .DeckList.GetCardByDeckId(action.EnergyCard.DeckId) as IEnergyCardLogic;
            var targetPokemon = _game.FindCardAnywhere(action.TargetPokemon) as IPokemonCardLogic;
            AttachEnergyFromHand(energyCard, targetPokemon);
            energyCard.Owner.PerformedOncePerTurnActions.Add(EnergyCard.ATTACHED_ENERGY_FOR_TURN);
            return Task.FromResult(action);
        }

        private static void AttachEnergyFromHand(
            IEnergyCardLogic energyCard,
            IPokemonCardLogic targetPokemon
        )
        {
            energyCard.Owner.Hand.RemoveCard(energyCard);
            targetPokemon.AttachEnergyCards(new() { energyCard });
        }

        public Task<DiscardAttachedEnergyCardsGA> Perform(DiscardAttachedEnergyCardsGA action)
        {
            action.Pokemon.DiscardEnergy(action.EnergyCards);
            return Task.FromResult(action);
        }

        public Task<DiscardAttachedEnergyCardsGA> Reperform(DiscardAttachedEnergyCardsGA action)
        {
            var pokemon = _game.FindCardAnywhere(action.Pokemon) as IPokemonCardLogic;
            var energyCards = _game
                .FindCardsAnywhere(action.EnergyCards.Cast<ICardLogic>().ToList())
                .Cast<IEnergyCardLogic>()
                .ToList();
            pokemon.DiscardEnergy(energyCards);
            return Task.FromResult(action);
        }

        public Task<BenchPokemonGA> Perform(BenchPokemonGA action)
        {
            BenchPokemon(action.Card);
            return Task.FromResult(action);
        }

        public Task<BenchPokemonGA> Reperform(BenchPokemonGA action)
        {
            BenchPokemon(_game.FindCardAnywhere(action.Card) as IPokemonCardLogic);
            return Task.FromResult(action);
        }

        private static void BenchPokemon(IPokemonCardLogic pokemon)
        {
            pokemon.Owner.Bench.AddCards(new() { pokemon });
            pokemon.Owner.Hand.RemoveCard(pokemon);
            pokemon.SetPutInPlay();
        }

        public Task<MovePokemonToBenchGA> Perform(MovePokemonToBenchGA action)
        {
            var pokemon = action.Pokemon;
            pokemon.Owner.Bench.AddCards(new() { pokemon });
            return Task.FromResult(action);
        }

        public Task<MovePokemonToBenchGA> Reperform(MovePokemonToBenchGA action)
        {
            var player = _game.GetPlayerByName(action.Pokemon.Owner.Name);
            var pokemon = player.DeckList.GetCardByDeckId(action.Pokemon.DeckId);
            player.Bench.AddCards(new() { pokemon });
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

        public Task<ResetPokemonTurnStateGA> Reperform(ResetPokemonTurnStateGA action)
        {
            var pokemonToReset = _game.FindCardAnywhere(action.PokemonToReset) as IPokemonCardLogic;
            pokemonToReset.PutIntoPlayThisTurn = false;
            pokemonToReset.AbilityUsedThisTurn = false;
            ActionSystem.INSTANCE.UnsubscribeFromGameAction<EndTurnGA>(
                pokemonToReset,
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

        public Task<RevealCardsFromDeckGA> Reperform(RevealCardsFromDeckGA action)
        {
            var player = _game.GetPlayerByName(action.Player.Name);
            var revealedCards = player.DeckList.GetCardsByDeckIds(action.RevealedCards);
            player.Deck.RemoveCards(revealedCards);
            return Task.FromResult(action);
        }

        public Task<TakeSelectionToHandGA> Perform(TakeSelectionToHandGA action)
        {
            action.Player.Hand.AddCards(action.SelectedCards);
            return Task.FromResult(action);
        }

        public Task<TakeSelectionToHandGA> Reperform(TakeSelectionToHandGA action)
        {
            var player = _game.GetPlayerByName(action.Player.Name);
            var selectedCards = player.DeckList.GetCardsByDeckIds(action.SelectedCards);
            player.Hand.AddCards(selectedCards);
            return Task.FromResult(action);
        }

        public Task<PutRemainingCardsUnderDeckGA> Perform(PutRemainingCardsUnderDeckGA action)
        {
            action.Player.Deck.AddCards(Shuffle(action.RemainingCards));
            return Task.FromResult(action);
        }

        public Task<PutRemainingCardsUnderDeckGA> Reperform(PutRemainingCardsUnderDeckGA action)
        {
            var player = _game.GetPlayerByName(action.Player.Name);
            var remainingCards = player.DeckList.GetCardsByDeckIds(action.RemainingCards);
            player.Deck.AddCards(Shuffle(remainingCards));
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
            return Task.FromResult(action);
        }

        public Task<SetActivePokemonGA> Perform(SetActivePokemonGA action)
        {
            SetActivePokemon(action.Card);
            return Task.FromResult(action);
        }

        public Task<SetActivePokemonGA> Reperform(SetActivePokemonGA action)
        {
            var card = _game
                .GetPlayerByName(action.Card.Owner.Name)
                .DeckList.GetCardByDeckId(action.Card.DeckId);
            SetActivePokemon(card as IPokemonCardLogic);
            _game.AdvanceGameStateQuietly();
            return Task.FromResult(action);
        }

        private static void SetActivePokemon(IPokemonCardLogic pokemon)
        {
            if (pokemon.Owner.ActivePokemon == null)
            {
                pokemon.Owner.ActivePokemon = pokemon;
                pokemon.Owner.Hand.RemoveCard(pokemon);
            }
            else
            {
                throw new Exception("There is already an active pokemon");
            }
        }

        public Task<DiscardCardsGA> Perform(DiscardCardsGA action)
        {
            foreach (var card in action.Cards)
            {
                card.Discard();
            }
            return Task.FromResult(action);
        }

        public Task<DiscardCardsGA> Reperform(DiscardCardsGA action)
        {
            foreach (var card in _game.FindCardsAnywhere(action.Cards))
            {
                card.Discard();
            }
            return Task.FromResult(action);
        }

        public async Task<SelectExactCardsGA> Perform(SelectExactCardsGA action)
        {
            var options = GetOptions(action.CardOptions.Cards, action.CardCondition);
            var selectedCards = await GetSelectedCards(
                action,
                options,
                _selectFromMap[action.Origin]
            );
            action.CardOptions.RemoveCards(selectedCards);
            if (action.Origin == SelectedCardsOrigin.Deck)
                action.Player.Deck.Shuffle();

            action.SelectedCards.AddRange(selectedCards);
            action.RemainingCards.AddRange(options.Except(selectedCards));

            return action;
        }

        private async Task<List<ICardLogic>> GetSelectedCards(
            SelectExactCardsGA action,
            List<ICardLogic> options,
            SelectFrom selectFrom
        )
        {
            return await _game.AwaitSelection(
                action.Player,
                options,
                list => list.Count == action.NumberOfCards,
                true,
                selectFrom
            );
        }

        public Task<SelectExactCardsGA> Reperform(SelectExactCardsGA action)
        {
            var player = _game.GetPlayerByName(action.Player.Name);
            var selectedCards = player.DeckList.GetCardsByDeckIds(action.SelectedCards);
            RemoveSelectedCardsFromOrigin(player, selectedCards, action.Origin);
            return Task.FromResult(action);
        }

        public async Task<SelectUpToCardsGA> Perform(SelectUpToCardsGA action)
        {
            var options = GetOptions(action.CardOptions.Cards, action.CardCondition);
            Debug.Log(
                $"Found {options.Count} options out of {action.CardOptions.Cards.Count} total cards"
            );
            var selectedCards = await GetSelectedCards(
                action,
                options,
                _selectFromMap[action.Origin]
            );
            action.CardOptions.RemoveCards(selectedCards);
            if (action.Origin == SelectedCardsOrigin.Deck)
                action.Player.Deck.Shuffle();

            action.SelectedCards.AddRange(selectedCards);
            action.RemainingCards.AddRange(options.Except(selectedCards));

            return action;
        }

        private async Task<List<ICardLogic>> GetSelectedCards(
            SelectUpToCardsGA action,
            List<ICardLogic> options,
            SelectFrom selectFrom
        )
        {
            return await _game.AwaitSelection(
                action.Player,
                options,
                list => list.Count <= action.Amount,
                false,
                selectFrom
            );
        }

        private static List<ICardLogic> GetOptions(
            List<ICardLogic> availableCards,
            Predicate<ICardLogic> cardCondition
        )
        {
            if (cardCondition is null)
                return availableCards;
            var options = new List<ICardLogic>();
            foreach (var card in availableCards)
            {
                if (cardCondition(card))
                    options.Add(card);
            }
            return options;
        }

        public Task<SelectUpToCardsGA> Reperform(SelectUpToCardsGA action)
        {
            var player = _game.GetPlayerByName(action.Player.Name);
            var selectedCards = player.DeckList.GetCardsByDeckIds(action.SelectedCards);
            RemoveSelectedCardsFromOrigin(player, selectedCards, action.Origin);
            return Task.FromResult(action);
        }

        private static void RemoveSelectedCardsFromOrigin(
            IPlayerLogic player,
            List<ICardLogic> selectedCards,
            SelectedCardsOrigin origin
        )
        {
            switch (origin)
            {
                case SelectedCardsOrigin.Hand:
                    player.Hand.RemoveCards(selectedCards);
                    break;
                case SelectedCardsOrigin.Deck:
                    player.Deck.RemoveCards(selectedCards);
                    break;
                case SelectedCardsOrigin.DiscardPile:
                    player.DiscardPile.RemoveCards(selectedCards);
                    break;
                case SelectedCardsOrigin.Other:
                    // No removal needed for 'Other' origin
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public Task<RemoveCardFromHandGA> Perform(RemoveCardFromHandGA action)
        {
            action.card.Owner.Hand.RemoveCard(action.card);
            return Task.FromResult(action);
        }

        public Task<RemoveCardFromHandGA> Reperform(RemoveCardFromHandGA action)
        {
            var player = _game.GetPlayerByName(action.card.Owner.Name);
            var card = player.DeckList.GetCardByDeckId(action.card.DeckId);
            player.Hand.RemoveCard(card);
            return Task.FromResult(action);
        }
    }
}
