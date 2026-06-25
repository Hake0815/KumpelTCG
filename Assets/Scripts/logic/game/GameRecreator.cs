using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using gamecore.card;
using gamecore.common;
using gamecore.serialization;
using Google.Protobuf.Collections;

namespace gamecore.game
{
    class GameRecreator
    {
        public static void RecreateGameFromGameState(
            ProtoBufGameState gameState,
            IReadOnlyDictionary<int, ProtoBufCardStatic> cardStatics,
            Game game
        )
        {
            game.Player1.Deck.Shuffle();
            game.Player2.Deck.Shuffle();
            RecreatePlayers(gameState, cardStatics, game);
            game.TurnCounter = game.Player1.TurnCounter + game.Player2.TurnCounter;
        }

        private static void RecreatePlayers(
            ProtoBufGameState gameState,
            IReadOnlyDictionary<int, ProtoBufCardStatic> cardStatics,
            Game game
        )
        {
            var isPlayer1Self =
                gameState.CardStates.First().Position.Owner == ProtoBufOwner.OwnerSelf;
            RecreatePlayersCards(gameState, cardStatics, game, isPlayer1Self);
            var player1State = isPlayer1Self ? gameState.SelfState : gameState.OpponentState;
            var player2State = isPlayer1Self ? gameState.OpponentState : gameState.SelfState;
            RecreatePlayerAttributes(player1State, game.Player1);
            RecreatePlayerAttributes(player2State, game.Player2);
        }

        private static void RecreatePlayerAttributes(
            ProtoBufPlayerState playerState,
            IPlayerLogic player
        )
        {
            player.IsActive = playerState.IsActive;
            player.IsAttacking = playerState.IsAttacking;
            player.TurnCounter = playerState.TurnCounter;
            if (playerState.KnowsHisPrizes)
            {
                player.Prizes.DeckSearched();
            }
            foreach (var trait in playerState.PlayerTurnTraits)
            {
                player.PlayerTurnTraits.Add(trait.FromProtoBuf());
            }
        }

        private static void RecreatePlayersCards(
            ProtoBufGameState gameState,
            IReadOnlyDictionary<int, ProtoBufCardStatic> cardStatics,
            Game game,
            bool isPlayer1Self
        )
        {
            RecreatePlayerCards(gameState, cardStatics, game.Player1, isPlayer1Self);
            RecreatePlayerCards(gameState, cardStatics, game.Player2, !isPlayer1Self);
        }

        private static void RecreatePlayerCards(
            ProtoBufGameState gameState,
            IReadOnlyDictionary<int, ProtoBufCardStatic> cardStatics,
            IPlayerLogic currentPlayer,
            bool isPlayerSelf
        )
        {
            SetTracker(
                isPlayerSelf,
                gameState,
                out int remainingHandCardsToSetup,
                out int remainingPrizesToSetup
            );
            currentPlayer.Deck.Cards.Sort(
                (left, right) =>
                    gameState
                        .CardStates[left.DeckId]
                        .Position.TopDeckPositionIndex.CompareTo(
                            gameState.CardStates[right.DeckId].Position.TopDeckPositionIndex
                        )
            );
            int currentCardIndex = 0;
            while (currentCardIndex < currentPlayer.Deck.CardCount)
            {
                var card = currentPlayer.Deck.Cards[currentCardIndex];
                var cardRemovedFromDeck = SetupCard(
                    card,
                    gameState,
                    cardStatics,
                    currentPlayer,
                    ref remainingHandCardsToSetup,
                    ref remainingPrizesToSetup
                );
                if (!cardRemovedFromDeck)
                {
                    currentCardIndex++;
                }
            }
        }

        private static void SetTracker(
            bool isPlayerSelf,
            ProtoBufGameState gameState,
            out int remainingHandCardsToSetup,
            out int remainingPrizesToSetup
        )
        {
            if (isPlayerSelf)
            {
                remainingHandCardsToSetup = gameState.SelfState.HandCount;
                remainingPrizesToSetup = gameState.SelfState.PrizesCount;
            }
            else
            {
                remainingHandCardsToSetup = gameState.OpponentState.HandCount;
                remainingPrizesToSetup = gameState.OpponentState.PrizesCount;
            }
        }

        private static bool SetupCard(
            ICardLogic card,
            ProtoBufGameState gameState,
            IReadOnlyDictionary<int, ProtoBufCardStatic> cardStatics,
            IPlayerLogic currentPlayer,
            ref int remainingHandCardsToSetup,
            ref int remainingPrizesToSetup
        )
        {
            var cardState = gameState.CardStates[card.DeckId];
            if (cardState.DeckId != card.DeckId)
            {
                throw new IllegalStateException(
                    $"card states in game state are not ordered by deck id"
                );
            }
            if (
                !cardStatics.TryGetValue(card.DeckId, out var cardStatic)
                || cardStatic.DeckId != card.DeckId
            )
            {
                throw new IllegalStateException(
                    $"Missing static data for card with deck id {card.DeckId}"
                );
            }

            card.OpponentPositionKnowledge =
                cardState.Position.OpponentPositionKnowledge.FromProtoBuf();
            card.OwnerPositionKnowledge = GetOwnerPositionKnowledge(
                cardState.Position.Owner == ProtoBufOwner.OwnerSelf,
                cardState.Position.PossiblePositions
            );
            card.TopDeckPositionIndex = Math.Max(0, cardState.Position.TopDeckPositionIndex);
            ApplyDynamicCardState(card, cardState);
            if (
                cardState.Position.PossiblePositions.Contains(ProtoBufCardPosition.CardPositionHand)
                && remainingHandCardsToSetup > 0
            )
            {
                currentPlayer.Deck.Cards.Remove(card);
                currentPlayer.Hand.AddCard(card);
                remainingHandCardsToSetup--;
                return true;
            }
            else if (
                cardState.Position.PossiblePositions.Contains(
                    ProtoBufCardPosition.CardPositionPrizes
                )
                && remainingPrizesToSetup > 0
            )
            {
                currentPlayer.Deck.Cards.Remove(card);
                currentPlayer.Prizes.AddCard(card);
                remainingPrizesToSetup--;
                return true;
            }
            else if (
                cardState.Position.PossiblePositions.Contains(
                    ProtoBufCardPosition.CardPositionAttachedToCard
                )
            )
            {
                AttachCard(currentPlayer, cardState, card);
                return true;
            }
            else if (
                cardState.Position.PossiblePositions.Contains(
                    ProtoBufCardPosition.CardPositionBench
                )
            )
            {
                currentPlayer.Deck.Cards.Remove(card);
                currentPlayer.Bench.AddCard(card);
                SetPokemonInPlayState(
                    card as IPokemonCardLogic,
                    cardStatic,
                    cardState.CardDynamic
                );
                return true;
            }
            else if (
                cardState.Position.PossiblePositions.Contains(
                    ProtoBufCardPosition.CardPositionActiveSpot
                )
            )
            {
                currentPlayer.Deck.Cards.Remove(card);
                currentPlayer.ActivePokemon = card as IPokemonCardLogic;
                SetPokemonInPlayState(
                    card as IPokemonCardLogic,
                    cardStatic,
                    cardState.CardDynamic
                );
                return true;
            }
            else if (
                cardState.Position.PossiblePositions.Contains(
                    ProtoBufCardPosition.CardPositionCurrentlyPlayed
                )
            )
            {
                currentPlayer.Deck.Cards.Remove(card);
                currentPlayer.CurrentlyPlayedCard = card;
                return true;
            }
            else if (
                cardState.Position.PossiblePositions.Contains(
                    ProtoBufCardPosition.CardPositionFloating
                )
            )
            {
                currentPlayer.Deck.Cards.Remove(card);
                currentPlayer.FloatingCards.Add(card);
                return true;
            }
            else if (
                cardState.Position.PossiblePositions.Contains(
                    ProtoBufCardPosition.CardPositionDiscardPile
                )
            )
            {
                currentPlayer.Deck.Cards.Remove(card);
                currentPlayer.DiscardPile.AddCard(card);
                return true;
            }
            else if (
                cardState.Position.PossiblePositions.Contains(ProtoBufCardPosition.CardPositionDeck)
            )
            {
                return false;
            }
            else
            {
                throw new IllegalStateException(
                    $"Card with deck id {card.DeckId} has invalid positions: {cardState.Position.PossiblePositions}"
                );
            }
        }

        private static void ApplyDynamicCardState(ICardLogic card, ProtoBufCardState cardState)
        {
            if (cardState.CardDynamic == null)
            {
                var isHidden =
                    cardState.Position.Owner == ProtoBufOwner.OwnerOpponent
                    && cardState.Position.PossiblePositions.Count == 3;
                if (!isHidden && card is not ITrainerCardLogic)
                {
                    throw new IllegalStateException(
                        $"Missing dynamic state for card with deck id {card.DeckId}"
                    );
                }
                return;
            }

            if (card is IPokemonCardLogic pokemon)
            {
                pokemon.MaxHp = cardState.CardDynamic.MaxHp;
                pokemon.Weakness = cardState.CardDynamic.Weakness.FromProtoBuf();
                pokemon.Resistance = cardState.CardDynamic.Resistance.FromProtoBuf();
                pokemon.RetreatCost = cardState.CardDynamic.RetreatCost;
                pokemon.NumberOfPrizeCardsOnKnockout =
                    cardState.CardDynamic.NumberOfPrizeCardsOnKnockout;
                pokemon.PokemonTurnTraits.Clear();
                pokemon.PokemonTurnTraits.AddRange(
                    cardState.CardDynamic.PokemonTurnTraits.Select(trait => trait.FromProtoBuf())
                );
                pokemon.TakeDamage(cardState.CardDynamic.CurrentDamage);
            }
            else if (card is IEnergyCardLogic energy)
            {
                energy.ProvidedEnergy.Clear();
                energy.ProvidedEnergy.AddRange(
                    cardState.CardDynamic.ProvidedEnergy.Select(type => type.FromProtoBuf())
                );
            }
        }

        private static void SetPokemonInPlayState(
            IPokemonCardLogic pokemon,
            ProtoBufCardStatic cardStatic,
            ProtoBufCardDynamic cardDynamic
        )
        {
            pokemon.PokemonType = cardStatic.EnergyType.FromProtoBuf();
            pokemon.PokemonTurnTraits.Clear();
            pokemon.PokemonTurnTraits.AddRange(
                cardDynamic.PokemonTurnTraits.Select(trait => trait.FromProtoBuf())
            );
        }

        private static void AttachCard(
            IPlayerLogic currentPlayer,
            ProtoBufCardState cardState,
            ICardLogic card
        )
        {
            currentPlayer.Deck.Cards.Remove(card);
            var attachedToCard =
                currentPlayer.DeckList.GetCardByDeckId(cardState.Position.AttachedToPokemonId)
                as IPokemonCardLogic;
            if (card is IPokemonCardLogic preEvolution)
            {
                attachedToCard.PreEvolutions.Add(preEvolution);
                attachedToCard.PreEvolutions.Sort(
                    (left, right) => right.Stage.CompareTo(left.Stage)
                );
            }
            else if (card is IEnergyCardLogic energyCard)
            {
                attachedToCard.AttachedEnergyCards.Add(energyCard);
            }
            else
            {
                throw new IllegalStateException($"Invalid card type: {card.GetType().Name}");
            }
        }

        private static PositionKnowledge GetOwnerPositionKnowledge(
            bool isOwnCard,
            RepeatedField<ProtoBufCardPosition> possiblePositions
        )
        {
            if (isOwnCard)
            {
                return possiblePositions.Count switch
                {
                    2 => PositionKnowledge.Unknown,
                    1 => PositionKnowledge.Known,
                    _ => throw new IllegalStateException(
                        $"Own card has invalid number of possible positions: {possiblePositions.Count}"
                    ),
                };
            }
            return possiblePositions.Count switch
            {
                3 => PositionKnowledge.Unknown,
                2 => possiblePositions.Contains(ProtoBufCardPosition.CardPositionHand)
                    ? PositionKnowledge.Known
                    : PositionKnowledge.Unknown,
                1 => PositionKnowledge.Known,
                _ => throw new IllegalStateException(
                    $"Opponent card has invalid number of possible positions: {possiblePositions.Count}"
                ),
            };
        }

        private static void SetTracker(
            ICardLogic card,
            ProtoBufCardState cardState,
            ProtoBufGameState gameState,
            out int remainingHandCardsToSetup,
            out int remainingPrizesToSetup
        )
        {
            var owner = card.Owner;
            var isSelf = cardState.Position.Owner == ProtoBufOwner.OwnerSelf;
            remainingHandCardsToSetup = isSelf
                ? gameState.SelfState.HandCount - owner.Hand.CardCount
                : gameState.OpponentState.HandCount - owner.Opponent.Hand.CardCount;
            remainingPrizesToSetup = isSelf
                ? gameState.SelfState.PrizesCount - owner.Prizes.CardCount
                : gameState.OpponentState.PrizesCount - owner.Opponent.Prizes.CardCount;
        }
    }
}
