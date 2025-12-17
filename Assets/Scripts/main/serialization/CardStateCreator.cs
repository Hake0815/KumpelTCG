using System;
using System.Collections.Generic;
using System.Linq;
using gamecore.card;
using gamecore.common;
using gamecore.game;
using Newtonsoft.Json;

namespace gamecore.serialization
{
    static class CardStateCreator
    {
        public static List<ProtoBufCardState> CreateCardStates(IPlayer player)
        {
            var cardStates = new List<ProtoBufCardState>();
            player.ActivePokemon?.Let(activePokemon =>
                AddActivePokemonAs(activePokemon, ProtoBufOwner.OwnerSelf, cardStates)
            );
            player.CurrentlyPlayedCard?.Let(currentlyPlayedCard =>
                AddCurrentlyPlayedCardAs(currentlyPlayedCard, ProtoBufOwner.OwnerSelf, cardStates)
            );
            player.FloatingCards?.Let(floatingCards =>
                AddFloatingCardsAsSelf(floatingCards, cardStates)
            );
            AddBenchedPokemonAs(player.Bench, ProtoBufOwner.OwnerSelf, cardStates);
            AddDiscardPileAs(player.DiscardPile, ProtoBufOwner.OwnerSelf, cardStates);
            AddPrizesAsSelf(player.Prizes, cardStates);
            AddHandAsSelf(player.Hand, cardStates);
            AddDeckAsSelf(player.Deck, cardStates);

            player.Opponent.ActivePokemon?.Let(activePokemon =>
                AddActivePokemonAs(activePokemon, ProtoBufOwner.OwnerOpponent, cardStates)
            );
            player.Opponent.CurrentlyPlayedCard?.Let(currentlyPlayedCard =>
                AddCurrentlyPlayedCardAs(
                    currentlyPlayedCard,
                    ProtoBufOwner.OwnerOpponent,
                    cardStates
                )
            );
            player.Opponent.FloatingCards?.Let(floatingCards =>
                AddFloatingCardsAsOpponent(floatingCards, cardStates)
            );
            AddBenchedPokemonAs(player.Opponent.Bench, ProtoBufOwner.OwnerOpponent, cardStates);
            AddDiscardPileAs(player.Opponent.DiscardPile, ProtoBufOwner.OwnerOpponent, cardStates);
            AddPrizesAsOpponent(player.Opponent.Prizes, cardStates);
            AddHandAsOpponent(player.Opponent.Hand, cardStates);
            AddDeckAsOpponent(player.Opponent.Deck, cardStates);
            cardStates = cardStates.OrderBy(cs => cs.Card.DeckId).ToList();
            if (cardStates.Count == 120)
            {
                GlobalLogger.Instance.Debug(
                    () =>
                        $"Card states are [{string.Join(", ", cardStates.Select(cs => JsonConvert.SerializeObject(cs)))}]"
                );
            }
            else
            {
                GlobalLogger.Instance.Error(
                    () =>
                        $"Card states count should be 120, but was {cardStates.Count}, card state were [{string.Join(", ", cardStates.Select(cs => JsonConvert.SerializeObject(cs)))}]"
                );
            }
            return cardStates;
        }

        private static void AddFloatingCardsAsSelf(
            List<ICard> floatingCards,
            List<ProtoBufCardState> cardStates
        )
        {
            foreach (var card in floatingCards)
            {
                cardStates.Add(
                    new ProtoBufCardState
                    {
                        Card = card.ToSerializable(),
                        Position = new ProtoBufPosition
                        {
                            Owner = ProtoBufOwner.OwnerSelf,
                            PossiblePositions = { ProtoBufCardPosition.CardPositionFloating },
                        },
                    }
                );
            }
        }

        private static void AddActivePokemonAs(
            IPokemonCard activePokemon,
            ProtoBufOwner owner,
            List<ProtoBufCardState> cardStates
        )
        {
            cardStates.Add(
                new ProtoBufCardState
                {
                    Card = activePokemon.ToSerializable(),
                    Position = new ProtoBufPosition
                    {
                        Owner = owner,
                        PossiblePositions = { ProtoBufCardPosition.CardPositionActiveSpot },
                    },
                }
            );
            AddAttachedCards(activePokemon, owner, cardStates);
        }

        private static void AddCurrentlyPlayedCardAs(
            ICard currentlyPlayedCard,
            ProtoBufOwner owner,
            List<ProtoBufCardState> cardStates
        )
        {
            cardStates.Add(
                new ProtoBufCardState
                {
                    Card = currentlyPlayedCard.ToSerializable(),
                    Position = new ProtoBufPosition
                    {
                        Owner = owner,
                        PossiblePositions = { ProtoBufCardPosition.CardPositionCurrentlyPlayed },
                    },
                }
            );
        }

        private static void AddBenchedPokemonAs(
            IBench bench,
            ProtoBufOwner owner,
            List<ProtoBufCardState> cardStates
        )
        {
            foreach (var card in bench.Cards)
            {
                cardStates.Add(
                    new ProtoBufCardState
                    {
                        Card = card.ToSerializable(),
                        Position = new ProtoBufPosition
                        {
                            Owner = owner,
                            PossiblePositions = { ProtoBufCardPosition.CardPositionBench },
                        },
                    }
                );
                AddAttachedCards(card as IPokemonCard, owner, cardStates);
            }
        }

        private static void AddAttachedCards(
            IPokemonCard pokemonCard,
            ProtoBufOwner owner,
            List<ProtoBufCardState> cardStates
        )
        {
            foreach (var card in pokemonCard.AttachedEnergyCards)
            {
                cardStates.Add(
                    new ProtoBufCardState
                    {
                        Card = card.ToSerializable(pokemonCard),
                        Position = new ProtoBufPosition
                        {
                            Owner = owner,
                            PossiblePositions = { ProtoBufCardPosition.CardPositionAttachedToCard },
                            AttachedToPokemonId = pokemonCard.DeckId,
                        },
                    }
                );
            }
            foreach (var card in pokemonCard.PreEvolutions)
            {
                cardStates.Add(
                    new ProtoBufCardState
                    {
                        Card = card.ToSerializable(pokemonCard),
                        Position = new ProtoBufPosition
                        {
                            Owner = owner,
                            PossiblePositions = { ProtoBufCardPosition.CardPositionAttachedToCard },
                            AttachedToPokemonId = pokemonCard.DeckId,
                        },
                    }
                );
            }
        }

        private static void AddDiscardPileAs(
            IDiscardPile discardPile,
            ProtoBufOwner owner,
            List<ProtoBufCardState> cardStates
        )
        {
            foreach (var card in discardPile.Cards)
            {
                cardStates.Add(
                    new ProtoBufCardState
                    {
                        Card = card.ToSerializable(),
                        Position = new ProtoBufPosition
                        {
                            Owner = owner,
                            PossiblePositions = { ProtoBufCardPosition.CardPositionDiscardPile },
                        },
                    }
                );
            }
        }

        private static void AddPrizesAsSelf(IPrizes prizes, List<ProtoBufCardState> cardStates)
        {
            foreach (var card in prizes.Cards)
            {
                if (card.OwnerPositionKnowledge == PositionKnowledge.Unknown)
                {
                    cardStates.Add(
                        new ProtoBufCardState
                        {
                            Card = card.ToSerializable(),
                            Position = new ProtoBufPosition
                            {
                                Owner = ProtoBufOwner.OwnerSelf,
                                PossiblePositions =
                                {
                                    ProtoBufCardPosition.CardPositionPrizes,
                                    ProtoBufCardPosition.CardPositionDeck,
                                },
                                TopDeckPositionIndex = card.TopDeckPositionIndex,
                            },
                        }
                    );
                }
                else
                {
                    cardStates.Add(
                        new ProtoBufCardState
                        {
                            Card = card.ToSerializable(),
                            Position = new ProtoBufPosition
                            {
                                Owner = ProtoBufOwner.OwnerSelf,
                                PossiblePositions = { ProtoBufCardPosition.CardPositionPrizes },
                            },
                        }
                    );
                }
            }
        }

        private static void AddPrizesAsOpponent(IPrizes prizes, List<ProtoBufCardState> cardStates)
        {
            foreach (var card in prizes.Cards)
            {
                if (card.OpponentPositionKnowledge == PositionKnowledge.Unknown)
                {
                    cardStates.Add(
                        new ProtoBufCardState
                        {
                            Card = ProtoBufUtil.CreateUnknownCard(card.DeckId),
                            Position = new ProtoBufPosition
                            {
                                Owner = ProtoBufOwner.OwnerOpponent,
                                PossiblePositions =
                                {
                                    ProtoBufCardPosition.CardPositionPrizes,
                                    ProtoBufCardPosition.CardPositionDeck,
                                    ProtoBufCardPosition.CardPositionHand,
                                },
                                TopDeckPositionIndex = card.TopDeckPositionIndex,
                            },
                        }
                    );
                }
                else
                {
                    cardStates.Add(
                        new ProtoBufCardState
                        {
                            Card = card.ToSerializable(),
                            Position = new ProtoBufPosition
                            {
                                Owner = ProtoBufOwner.OwnerOpponent,
                                PossiblePositions = { ProtoBufCardPosition.CardPositionPrizes },
                            },
                        }
                    );
                }
            }
        }

        private static void AddHandAsSelf(IHand hand, List<ProtoBufCardState> cardStates)
        {
            foreach (var card in hand.Cards)
            {
                cardStates.Add(
                    new ProtoBufCardState
                    {
                        Card = card.ToSerializable(),
                        Position = new ProtoBufPosition
                        {
                            Owner = ProtoBufOwner.OwnerSelf,
                            PossiblePositions = { ProtoBufCardPosition.CardPositionHand },
                        },
                    }
                );
            }
        }

        private static void AddHandAsOpponent(IHand hand, List<ProtoBufCardState> cardStates)
        {
            foreach (var card in hand.Cards)
            {
                switch (card.OpponentPositionKnowledge)
                {
                    case PositionKnowledge.Known:
                        cardStates.Add(
                            new ProtoBufCardState
                            {
                                Card = card.ToSerializable(),
                                Position = new ProtoBufPosition
                                {
                                    Owner = ProtoBufOwner.OwnerOpponent,
                                    PossiblePositions = { ProtoBufCardPosition.CardPositionHand },
                                },
                            }
                        );
                        break;
                    case PositionKnowledge.NotPrized:
                        cardStates.Add(
                            new ProtoBufCardState
                            {
                                Card = card.ToSerializable(),
                                Position = new ProtoBufPosition
                                {
                                    Owner = ProtoBufOwner.OwnerOpponent,
                                    PossiblePositions =
                                    {
                                        ProtoBufCardPosition.CardPositionHand,
                                        ProtoBufCardPosition.CardPositionDeck,
                                    },
                                    TopDeckPositionIndex = card.TopDeckPositionIndex,
                                },
                            }
                        );
                        break;
                    case PositionKnowledge.Unknown:
                        cardStates.Add(
                            new ProtoBufCardState
                            {
                                Card = ProtoBufUtil.CreateUnknownCard(card.DeckId),
                                Position = new ProtoBufPosition
                                {
                                    Owner = ProtoBufOwner.OwnerOpponent,
                                    PossiblePositions =
                                    {
                                        ProtoBufCardPosition.CardPositionHand,
                                        ProtoBufCardPosition.CardPositionDeck,
                                        ProtoBufCardPosition.CardPositionPrizes,
                                    },
                                    TopDeckPositionIndex = card.TopDeckPositionIndex,
                                },
                            }
                        );
                        break;
                }
            }
        }

        private static void AddDeckAsSelf(IDeck deck, List<ProtoBufCardState> cardStates)
        {
            foreach (var card in deck.Cards)
            {
                if (card.OwnerPositionKnowledge == PositionKnowledge.Known)
                {
                    cardStates.Add(
                        new ProtoBufCardState
                        {
                            Card = card.ToSerializable(),
                            Position = new ProtoBufPosition
                            {
                                Owner = ProtoBufOwner.OwnerSelf,
                                PossiblePositions = { ProtoBufCardPosition.CardPositionDeck },
                                TopDeckPositionIndex = card.TopDeckPositionIndex,
                            },
                        }
                    );
                }
                else
                {
                    cardStates.Add(
                        new ProtoBufCardState
                        {
                            Card = card.ToSerializable(),
                            Position = new ProtoBufPosition
                            {
                                Owner = ProtoBufOwner.OwnerSelf,
                                PossiblePositions =
                                {
                                    ProtoBufCardPosition.CardPositionDeck,
                                    ProtoBufCardPosition.CardPositionPrizes,
                                },
                                TopDeckPositionIndex = card.TopDeckPositionIndex,
                            },
                        }
                    );
                }
            }
        }

        private static void AddDeckAsOpponent(IDeck deck, List<ProtoBufCardState> cardStates)
        {
            foreach (var card in deck.Cards)
            {
                switch (card.OpponentPositionKnowledge)
                {
                    case PositionKnowledge.Known:
                        cardStates.Add(
                            new ProtoBufCardState
                            {
                                Card = card.ToSerializable(),
                                Position = new ProtoBufPosition
                                {
                                    Owner = ProtoBufOwner.OwnerOpponent,
                                    PossiblePositions = { ProtoBufCardPosition.CardPositionDeck },
                                    TopDeckPositionIndex = card.TopDeckPositionIndex,
                                },
                            }
                        );
                        break;
                    case PositionKnowledge.NotPrized:
                        cardStates.Add(
                            new ProtoBufCardState
                            {
                                Card = card.ToSerializable(),
                                Position = new ProtoBufPosition
                                {
                                    Owner = ProtoBufOwner.OwnerOpponent,
                                    PossiblePositions =
                                    {
                                        ProtoBufCardPosition.CardPositionHand,
                                        ProtoBufCardPosition.CardPositionDeck,
                                    },
                                    TopDeckPositionIndex = card.TopDeckPositionIndex,
                                },
                            }
                        );
                        break;
                    case PositionKnowledge.Unknown:
                        cardStates.Add(
                            new ProtoBufCardState
                            {
                                Card = ProtoBufUtil.CreateUnknownCard(card.DeckId),
                                Position = new ProtoBufPosition
                                {
                                    Owner = ProtoBufOwner.OwnerOpponent,
                                    PossiblePositions =
                                    {
                                        ProtoBufCardPosition.CardPositionHand,
                                        ProtoBufCardPosition.CardPositionDeck,
                                        ProtoBufCardPosition.CardPositionPrizes,
                                    },
                                    TopDeckPositionIndex = card.TopDeckPositionIndex,
                                },
                            }
                        );
                        break;
                }
            }
        }

        private static void AddFloatingCardsAsOpponent(
            List<ICard> floatingCards,
            List<ProtoBufCardState> cardStates
        )
        {
            foreach (var card in floatingCards)
            {
                switch (card.OpponentPositionKnowledge)
                {
                    case PositionKnowledge.Known:
                        cardStates.Add(
                            new ProtoBufCardState
                            {
                                Card = card.ToSerializable(),
                                Position = new ProtoBufPosition
                                {
                                    Owner = ProtoBufOwner.OwnerOpponent,
                                    PossiblePositions =
                                    {
                                        ProtoBufCardPosition.CardPositionFloating,
                                    },
                                },
                            }
                        );
                        break;
                    case PositionKnowledge.NotPrized:
                        cardStates.Add(
                            new ProtoBufCardState
                            {
                                Card = card.ToSerializable(),
                                Position = new ProtoBufPosition
                                {
                                    Owner = ProtoBufOwner.OwnerOpponent,
                                    PossiblePositions =
                                    {
                                        ProtoBufCardPosition.CardPositionHand,
                                        ProtoBufCardPosition.CardPositionDeck,
                                    },
                                    TopDeckPositionIndex = card.TopDeckPositionIndex,
                                },
                            }
                        );
                        break;
                    case PositionKnowledge.Unknown:
                        cardStates.Add(
                            new ProtoBufCardState
                            {
                                Card = ProtoBufUtil.CreateUnknownCard(card.DeckId),
                                Position = new ProtoBufPosition
                                {
                                    Owner = ProtoBufOwner.OwnerOpponent,
                                    PossiblePositions =
                                    {
                                        ProtoBufCardPosition.CardPositionHand,
                                        ProtoBufCardPosition.CardPositionDeck,
                                        ProtoBufCardPosition.CardPositionPrizes,
                                    },
                                    TopDeckPositionIndex = card.TopDeckPositionIndex,
                                },
                            }
                        );
                        break;
                }
            }
        }
    }
}
