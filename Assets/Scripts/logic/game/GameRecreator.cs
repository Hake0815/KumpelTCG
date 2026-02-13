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
        public static void RecreateGameFromGameState(ProtoBufGameState gameState, Game game)
        {
            game.Player1.Deck.Shuffle();
            game.Player2.Deck.Shuffle();
            RecreatePlayers(gameState, game);
            game.TurnCounter = game.Player1.TurnCounter + game.Player2.TurnCounter;
        }

        private static void RecreatePlayers(ProtoBufGameState gameState, Game game)
        {
            var isPlayer1Self =
                gameState.CardStates.First().Position.Owner == ProtoBufOwner.OwnerSelf;
            RecreatePlayersCards(gameState, game, isPlayer1Self);
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
            foreach (var trait in playerState.PlayerTurnTraits)
            {
                player.PlayerTurnTraits.Add(trait.FromProtoBuf());
            }
        }

        private static void RecreatePlayersCards(
            ProtoBufGameState gameState,
            Game game,
            bool isPlayer1Self
        )
        {
            RecreatePlayerCards(gameState, game.Player1, isPlayer1Self);
            RecreatePlayerCards(gameState, game.Player2, !isPlayer1Self);
        }

        private static void RecreatePlayerCards(
            ProtoBufGameState gameState,
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
            bool cardRemovedFromDeck = false;
            while (currentCardIndex < currentPlayer.Deck.CardCount)
            {
                var card = currentPlayer.Deck.Cards[currentCardIndex];
                SetupCard(
                    card,
                    gameState,
                    currentPlayer,
                    ref remainingHandCardsToSetup,
                    ref remainingPrizesToSetup,
                    ref cardRemovedFromDeck
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

        private static void SetupCard(
            ICardLogic card,
            ProtoBufGameState gameState,
            IPlayerLogic currentPlayer,
            ref int remainingHandCardsToSetup,
            ref int remainingPrizesToSetup,
            ref bool cardRemovedFromDeck
        )
        {
            var cardState = gameState.CardStates[card.DeckId];
            if (cardState.Card.DeckId != card.DeckId)
            {
                throw new IllegalStateException(
                    $"card states in game state are not ordered by deck id"
                );
            }

            card.OpponentPositionKnowledge =
                cardState.Position.OpponentPositionKnowledge.FromProtoBuf();
            card.OwnerPositionKnowledge = GetOwnerPositionKnowledge(
                cardState.Position.PossiblePositions
            );
            card.TopDeckPositionIndex = Math.Max(0, cardState.Position.TopDeckPositionIndex);
            if (
                cardState.Position.PossiblePositions.Contains(ProtoBufCardPosition.CardPositionHand)
                && remainingHandCardsToSetup > 0
            )
            {
                currentPlayer.Deck.Cards.Remove(card);
                cardRemovedFromDeck = true;
                currentPlayer.Hand.AddCard(card);
                remainingHandCardsToSetup--;
            }
            else if (
                cardState.Position.PossiblePositions.Contains(
                    ProtoBufCardPosition.CardPositionPrizes
                )
                && remainingPrizesToSetup > 0
            )
            {
                currentPlayer.Deck.Cards.Remove(card);
                cardRemovedFromDeck = true;
                currentPlayer.Prizes.AddCard(card);
                remainingPrizesToSetup--;
            }
            else if (
                cardState.Position.PossiblePositions.Contains(
                    ProtoBufCardPosition.CardPositionAttachedToCard
                )
            )
            {
                AttachCard(currentPlayer, cardState, card);
            }
            else if (
                cardState.Position.PossiblePositions.Contains(
                    ProtoBufCardPosition.CardPositionBench
                )
            )
            {
                currentPlayer.Deck.Cards.Remove(card);
                cardRemovedFromDeck = true;
                currentPlayer.Bench.AddCard(card);
                SetPokemonInPlayState(card as IPokemonCardLogic, cardState);
            }
            else if (
                cardState.Position.PossiblePositions.Contains(
                    ProtoBufCardPosition.CardPositionActiveSpot
                )
            )
            {
                currentPlayer.Deck.Cards.Remove(card);
                cardRemovedFromDeck = true;
                currentPlayer.ActivePokemon = card as IPokemonCardLogic;
                SetPokemonInPlayState(card as IPokemonCardLogic, cardState);
            }
            else if (
                cardState.Position.PossiblePositions.Contains(
                    ProtoBufCardPosition.CardPositionCurrentlyPlayed
                )
            )
            {
                currentPlayer.Deck.Cards.Remove(card);
                cardRemovedFromDeck = true;
                currentPlayer.CurrentlyPlayedCard = card;
            }
            else if (
                cardState.Position.PossiblePositions.Contains(
                    ProtoBufCardPosition.CardPositionFloating
                )
            )
            {
                currentPlayer.Deck.Cards.Remove(card);
                cardRemovedFromDeck = true;
                currentPlayer.FloatingCards.Add(card);
            }
            else if (
                cardState.Position.PossiblePositions.Contains(
                    ProtoBufCardPosition.CardPositionDiscardPile
                )
            )
            {
                currentPlayer.Deck.Cards.Remove(card);
                cardRemovedFromDeck = true;
                currentPlayer.DiscardPile.AddCard(card);
            }
            else if (
                cardState.Position.PossiblePositions.Contains(ProtoBufCardPosition.CardPositionDeck)
            )
            {
                cardRemovedFromDeck = false;
            }
            else
            {
                throw new IllegalStateException(
                    $"Card with deck id {card.DeckId} has invalid positions: {cardState.Position.PossiblePositions}"
                );
            }
        }

        private static void SetPokemonInPlayState(
            IPokemonCardLogic pokemon,
            ProtoBufCardState cardState
        )
        {
            pokemon.PokemonType = cardState.Card.EnergyType.FromProtoBuf();
            pokemon.Weakness = cardState.Card.Weakness.FromProtoBuf();
            pokemon.Resistance = cardState.Card.Resistance.FromProtoBuf();
            pokemon.NumberOfPrizeCardsOnKnockout = cardState.Card.NumberOfPrizeCardsOnKnockout;
            pokemon.PokemonTurnTraits.AddRange(
                cardState.Card.PokemonTurnTraits.Select(trait => trait.FromProtoBuf())
            );
            pokemon.TakeDamage(cardState.Card.CurrentDamage);
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
            RepeatedField<ProtoBufCardPosition> possiblePositions
        )
        {
            return possiblePositions.Contains(ProtoBufCardPosition.CardPositionPrizes)
                ? PositionKnowledge.Unknown
                : PositionKnowledge.Known;
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
