using System;
using System.Collections.Generic;
using System.Linq;
using gamecore.card;
using gamecore.common;

namespace gamecore.game
{
    static class CardStateCreator
    {
        public static List<CardStateJson> CreateCardStates(IPlayerLogic player)
        {
            var cardStates = new List<CardStateJson>();
            player.ActivePokemon?.Let(activePokemon =>
                AddActivePokemonAs(activePokemon, Owner.Self, cardStates)
            );
            player.CurrentlyPlayedCard?.Let(currentlyPlayedCard =>
                AddCurrentlyPlayedCardAs(currentlyPlayedCard, Owner.Self, cardStates)
            );
            player.FloatingCards?.Let(floatingCards =>
                AddFloatingCardsAsSelf(floatingCards, cardStates)
            );
            AddBenchedPokemonAs(player.Bench, Owner.Self, cardStates);
            AddDiscardPileAs(player.DiscardPile, Owner.Self, cardStates);
            AddPrizesAsSelf(player.Prizes, cardStates);
            AddHandAsSelf(player.Hand, cardStates);
            AddDeckAsSelf(player.Deck, cardStates);

            player.Opponent.ActivePokemon?.Let(activePokemon =>
                AddActivePokemonAs(activePokemon, Owner.Opponent, cardStates)
            );
            player.Opponent.CurrentlyPlayedCard?.Let(currentlyPlayedCard =>
                AddCurrentlyPlayedCardAs(currentlyPlayedCard, Owner.Opponent, cardStates)
            );
            player.Opponent.FloatingCards?.Let(floatingCards =>
                AddFloatingCardsAsOpponent(floatingCards, cardStates)
            );
            AddBenchedPokemonAs(player.Opponent.Bench, Owner.Opponent, cardStates);
            AddDiscardPileAs(player.Opponent.DiscardPile, Owner.Opponent, cardStates);
            AddPrizesAsOpponent(player.Opponent.Prizes, cardStates);
            AddHandAsOpponent(player.Opponent.Hand, cardStates);
            AddDeckAsOpponent(player.Opponent.Deck, cardStates);
            if (cardStates.Count == 120)
            {
                GlobalLogger.Instance.Debug(
                    $"Card states are [{string.Join(", ", cardStates.OrderBy(cs => cs.Card.DeckId).Select(cs => cs.ToJsonString()))}]"
                );
            }
            else
            {
                GlobalLogger.Instance.Error(
                    $"Card states count should be 120, but was {cardStates.Count}, card state were [{string.Join(", ", cardStates.OrderBy(cs => cs.Card.DeckId).Select(cs => cs.ToJsonString()))}]"
                );
            }
            return cardStates;
        }

        private static void AddFloatingCardsAsSelf(
            List<ICardLogic> floatingCards,
            List<CardStateJson> cardStates
        )
        {
            foreach (var card in floatingCards)
            {
                cardStates.Add(
                    new CardStateJson(
                        card.ToSerializable(),
                        new PositionJson(Owner.Self, new() { "floating" })
                    )
                );
            }
        }

        private static void AddActivePokemonAs(
            IPokemonCardLogic activePokemon,
            Owner owner,
            List<CardStateJson> cardStates
        )
        {
            cardStates.Add(
                new CardStateJson(
                    activePokemon.ToSerializable(),
                    new PositionJson(owner, new() { "active_spot" })
                )
            );
            AddAttachedCards(activePokemon, owner, cardStates);
        }

        private static void AddCurrentlyPlayedCardAs(
            ICardLogic currentlyPlayedCard,
            Owner owner,
            List<CardStateJson> cardStates
        )
        {
            cardStates.Add(
                new CardStateJson(
                    currentlyPlayedCard.ToSerializable(),
                    new PositionJson(owner, new() { "currently_played" })
                )
            );
        }

        private static void AddBenchedPokemonAs(
            IBench bench,
            Owner owner,
            List<CardStateJson> cardStates
        )
        {
            foreach (var card in bench.Cards)
            {
                cardStates.Add(
                    new CardStateJson(
                        card.ToSerializable(),
                        new PositionJson(owner, new() { "bench" })
                    )
                );
                AddAttachedCards(card as IPokemonCard, owner, cardStates);
            }
        }

        private static void AddAttachedCards(
            IPokemonCard pokemonCard,
            Owner owner,
            List<CardStateJson> cardStates
        )
        {
            foreach (var card in pokemonCard.AttachedEnergyCards)
            {
                cardStates.Add(
                    new CardStateJson(
                        card.ToSerializable(pokemonCard),
                        new PositionJson(owner, attachedToPokemonId: pokemonCard.DeckId)
                    )
                );
            }
            foreach (var card in pokemonCard.PreEvolutions)
            {
                cardStates.Add(
                    new CardStateJson(
                        card.ToSerializable(pokemonCard),
                        new PositionJson(owner, attachedToPokemonId: pokemonCard.DeckId)
                    )
                );
            }
        }

        private static void AddDiscardPileAs(
            IDiscardPile discardPile,
            Owner owner,
            List<CardStateJson> cardStates
        )
        {
            foreach (var card in discardPile.Cards)
            {
                cardStates.Add(
                    new CardStateJson(
                        card.ToSerializable(),
                        new PositionJson(owner, new() { "discard_pile" })
                    )
                );
            }
        }

        private static void AddPrizesAsSelf(IPrizes prizes, List<CardStateJson> cardStates)
        {
            foreach (var card in prizes.Cards)
            {
                if (card.OwnerPositionKnowledge == PositionKnowledge.Unknown)
                {
                    cardStates.Add(
                        new CardStateJson(
                            card.ToSerializable(),
                            new PositionJson(
                                Owner.Self,
                                new() { "prizes", "deck" },
                                topDeckPositionIndex: card.TopDeckPositionIndex
                            )
                        )
                    );
                }
                else
                {
                    cardStates.Add(
                        new CardStateJson(
                            card.ToSerializable(),
                            new PositionJson(Owner.Self, new() { "prizes" })
                        )
                    );
                }
            }
        }

        private static void AddPrizesAsOpponent(IPrizes prizes, List<CardStateJson> cardStates)
        {
            foreach (var card in prizes.Cards)
            {
                if (card.OpponentPositionKnowledge == PositionKnowledge.Unknown)
                {
                    cardStates.Add(
                        new CardStateJson(
                            CardJson.CreateUnknownCard(card.DeckId),
                            new PositionJson(
                                Owner.Opponent,
                                new() { "prizes", "deck", "hand" },
                                topDeckPositionIndex: card.TopDeckPositionIndex
                            )
                        )
                    );
                }
                else
                {
                    cardStates.Add(
                        new CardStateJson(
                            card.ToSerializable(),
                            new PositionJson(Owner.Opponent, new() { "prizes" })
                        )
                    );
                }
            }
        }

        private static void AddHandAsSelf(IHand hand, List<CardStateJson> cardStates)
        {
            foreach (var card in hand.Cards)
            {
                cardStates.Add(
                    new CardStateJson(
                        card.ToSerializable(),
                        new PositionJson(Owner.Self, new() { "hand" })
                    )
                );
            }
        }

        private static void AddHandAsOpponent(IHand hand, List<CardStateJson> cardStates)
        {
            foreach (var card in hand.Cards)
            {
                switch (card.OpponentPositionKnowledge)
                {
                    case PositionKnowledge.Known:
                        cardStates.Add(
                            new CardStateJson(
                                card.ToSerializable(),
                                new PositionJson(Owner.Opponent, new() { "hand" })
                            )
                        );
                        break;
                    case PositionKnowledge.NotPrized:
                        cardStates.Add(
                            new CardStateJson(
                                card.ToSerializable(),
                                new PositionJson(
                                    Owner.Opponent,
                                    new() { "hand", "deck" },
                                    topDeckPositionIndex: card.TopDeckPositionIndex
                                )
                            )
                        );
                        break;
                    case PositionKnowledge.Unknown:
                        cardStates.Add(
                            new CardStateJson(
                                CardJson.CreateUnknownCard(card.DeckId),
                                new PositionJson(
                                    Owner.Opponent,
                                    new() { "hand", "deck", "prizes" },
                                    topDeckPositionIndex: card.TopDeckPositionIndex
                                )
                            )
                        );
                        break;
                }
            }
        }

        private static void AddDeckAsSelf(IDeck deck, List<CardStateJson> cardStates)
        {
            foreach (var card in deck.Cards)
            {
                if (card.OwnerPositionKnowledge == PositionKnowledge.Known)
                {
                    cardStates.Add(
                        new CardStateJson(
                            card.ToSerializable(),
                            new PositionJson(
                                Owner.Self,
                                new() { "deck" },
                                topDeckPositionIndex: card.TopDeckPositionIndex
                            )
                        )
                    );
                }
                else
                {
                    cardStates.Add(
                        new CardStateJson(
                            card.ToSerializable(),
                            new PositionJson(
                                Owner.Self,
                                new() { "deck", "prizes" },
                                topDeckPositionIndex: card.TopDeckPositionIndex
                            )
                        )
                    );
                }
            }
        }

        private static void AddDeckAsOpponent(IDeck deck, List<CardStateJson> cardStates)
        {
            foreach (var card in deck.Cards)
            {
                switch (card.OpponentPositionKnowledge)
                {
                    case PositionKnowledge.Known:
                        cardStates.Add(
                            new CardStateJson(
                                card.ToSerializable(),
                                new PositionJson(
                                    Owner.Opponent,
                                    new() { "deck" },
                                    topDeckPositionIndex: card.TopDeckPositionIndex
                                )
                            )
                        );
                        break;
                    case PositionKnowledge.NotPrized:
                        cardStates.Add(
                            new CardStateJson(
                                card.ToSerializable(),
                                new PositionJson(
                                    Owner.Opponent,
                                    new() { "hand", "deck" },
                                    topDeckPositionIndex: card.TopDeckPositionIndex
                                )
                            )
                        );
                        break;
                    case PositionKnowledge.Unknown:
                        cardStates.Add(
                            new CardStateJson(
                                CardJson.CreateUnknownCard(card.DeckId),
                                new PositionJson(
                                    Owner.Opponent,
                                    new() { "hand", "deck", "prizes" },
                                    topDeckPositionIndex: card.TopDeckPositionIndex
                                )
                            )
                        );
                        break;
                }
            }
        }

        private static void AddFloatingCardsAsOpponent(
            List<ICardLogic> floatingCards,
            List<CardStateJson> cardStates
        )
        {
            foreach (var card in floatingCards)
            {
                switch (card.OpponentPositionKnowledge)
                {
                    case PositionKnowledge.Known:
                        cardStates.Add(
                            new CardStateJson(
                                card.ToSerializable(),
                                new PositionJson(Owner.Opponent, new() { "floating" })
                            )
                        );
                        break;
                    case PositionKnowledge.NotPrized:
                        cardStates.Add(
                            new CardStateJson(
                                card.ToSerializable(),
                                new PositionJson(
                                    Owner.Opponent,
                                    new() { "hand", "deck" },
                                    topDeckPositionIndex: card.TopDeckPositionIndex
                                )
                            )
                        );
                        break;
                    case PositionKnowledge.Unknown:
                        cardStates.Add(
                            new CardStateJson(
                                CardJson.CreateUnknownCard(card.DeckId),
                                new PositionJson(
                                    Owner.Opponent,
                                    new() { "hand", "deck", "prizes" },
                                    topDeckPositionIndex: card.TopDeckPositionIndex
                                )
                            )
                        );
                        break;
                }
            }
        }
    }
}
