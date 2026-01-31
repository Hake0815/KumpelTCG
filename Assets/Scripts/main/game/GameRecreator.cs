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
            RecreatePlayerCards(gameState, game);
            var isPlayer1Self =
                gameState.CardStates.First().Position.Owner == ProtoBufOwner.OwnerSelf;
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

        private static void RecreatePlayerCards(ProtoBufGameState gameState, Game game)
        {
            int remainingHandCardsToSetup = 0;
            int remainingPrizesToSetup = 0;
            IPlayerLogic currentPlayer = null;

            foreach (var cardState in gameState.CardStates)
            {
                SetupCard(
                    gameState,
                    game,
                    ref remainingHandCardsToSetup,
                    ref remainingPrizesToSetup,
                    ref currentPlayer,
                    cardState
                );
            }
            game.Player1.Deck.Cards.Sort(
                (left, right) => left.TopDeckPositionIndex.CompareTo(right.TopDeckPositionIndex)
            );
            game.Player2.Deck.Cards.Sort(
                (left, right) => left.TopDeckPositionIndex.CompareTo(right.TopDeckPositionIndex)
            );
        }

        private static void SetupCard(
            ProtoBufGameState gameState,
            Game game,
            ref int remainingHandCardsToSetup,
            ref int remainingPrizesToSetup,
            ref IPlayerLogic currentPlayer,
            ProtoBufCardState cardState
        )
        {
            if (cardState.Card.DeckId == 0 || cardState.Card.DeckId == 60)
            {
                SetTracker(
                    cardState,
                    gameState,
                    out remainingHandCardsToSetup,
                    out remainingPrizesToSetup
                );
                currentPlayer = cardState.Card.DeckId == 0 ? game.Player1 : game.Player2;
            }
            var card = currentPlayer.DeckList.GetCardByDeckId(cardState.Card.DeckId);
            card.OpponentPositionKnowledge =
                cardState.Position.OpponentPositionKnowledge.FromProtoBuf();
            card.OwnerPositionKnowledge = GetOwnerPositionKnowledge(
                cardState.Position.PossiblePositions
            );
            card.TopDeckPositionIndex = cardState.Position.TopDeckPositionIndex;
            if (
                cardState.Position.PossiblePositions.Contains(ProtoBufCardPosition.CardPositionHand)
                && remainingHandCardsToSetup > 0
            )
            {
                currentPlayer.Deck.RemoveCard(card);
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
                currentPlayer.Deck.RemoveCard(card);
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
                currentPlayer.Deck.RemoveCard(card);
                currentPlayer.Bench.AddCard(card);
                SetPokemonInPlayState(card as IPokemonCardLogic, cardState);
            }
            else if (
                cardState.Position.PossiblePositions.Contains(
                    ProtoBufCardPosition.CardPositionActiveSpot
                )
            )
            {
                currentPlayer.Deck.RemoveCard(card);
                currentPlayer.ActivePokemon = card as IPokemonCardLogic;
                SetPokemonInPlayState(card as IPokemonCardLogic, cardState);
            }
            else if (
                cardState.Position.PossiblePositions.Contains(
                    ProtoBufCardPosition.CardPositionCurrentlyPlayed
                )
            )
            {
                currentPlayer.Deck.RemoveCard(card);
                currentPlayer.CurrentlyPlayedCard = card;
            }
            else if (
                cardState.Position.PossiblePositions.Contains(
                    ProtoBufCardPosition.CardPositionFloating
                )
            )
            {
                currentPlayer.Deck.RemoveCard(card);
                currentPlayer.FloatingCards.Add(card);
            }
            else if (
                cardState.Position.PossiblePositions.Contains(
                    ProtoBufCardPosition.CardPositionDiscardPile
                )
            )
            {
                currentPlayer.Deck.RemoveCard(card);
                currentPlayer.DiscardPile.AddCard(card);
            }
            else if (
                cardState.Position.PossiblePositions.Contains(ProtoBufCardPosition.CardPositionDeck)
            )
            { /* Deck is already setup */
            }
            else
            {
                throw new IllegalStateException(
                    $"Invalid position: {cardState.Position.PossiblePositions}"
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
            currentPlayer.Deck.RemoveCard(card);
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
            return possiblePositions.Count switch
            {
                3 => PositionKnowledge.Unknown,
                2 => PositionKnowledge.NotPrized,
                1 => PositionKnowledge.Known,
                _ => throw new IllegalStateException(
                    $"Invalid number of possible positions: {possiblePositions.Count}"
                ),
            };
        }

        private static void SetTracker(
            ProtoBufCardState cardState,
            ProtoBufGameState gameState,
            out int remainingHandCardsToSetup,
            out int remainingPrizesToSetup
        )
        {
            var isSelf = cardState.Position.Owner == ProtoBufOwner.OwnerSelf;
            remainingHandCardsToSetup = isSelf
                ? gameState.SelfState.HandCount
                : gameState.OpponentState.HandCount;
            remainingPrizesToSetup = isSelf
                ? gameState.SelfState.PrizesCount
                : gameState.OpponentState.PrizesCount;
        }
    }
}
