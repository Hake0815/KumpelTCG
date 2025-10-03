using System.Collections.Generic;
using gamecore.card;

namespace gamecore.game
{
    class CardStateCreator
    {
        public static List<CardStateJson> CreateCardStates(IPlayerLogic player)
        {
            var cardStates = new List<CardStateJson>();
            AddActivePokemonAs(player, Owner.Self, cardStates);
            AddBenchedPokemonAs(player, Owner.Self, cardStates);
            AddDiscardPileAs(player, Owner.Self, cardStates);
            AddPrizesAsSelf(player, cardStates);
            AddHandAsSelf(player, cardStates);
            AddDeckAsSelf(player, cardStates);

            AddActivePokemonAs(player.Opponent, Owner.Opponent, cardStates);
            AddBenchedPokemonAs(player.Opponent, Owner.Opponent, cardStates);
            AddDiscardPileAs(player.Opponent, Owner.Opponent, cardStates);
            AddPrizesAsOpponent(player.Opponent, cardStates);
            AddHandAsOpponent(player.Opponent, cardStates);
            AddDeckAsOpponent(player.Opponent, cardStates);
            return cardStates;
        }

        private static void AddActivePokemonAs(
            IPlayer player,
            Owner owner,
            List<CardStateJson> cardStates
        )
        {
            cardStates.Add(
                new CardStateJson(
                    player.ActivePokemon.ToSerializable(),
                    new PositionJson(owner, new() { "active_spot" })
                )
            );
            AddAttachedCards(player.ActivePokemon, owner, cardStates);
        }

        private static void AddBenchedPokemonAs(
            IPlayer player,
            Owner self,
            List<CardStateJson> cardStates
        )
        {
            foreach (var card in player.Bench.Cards)
            {
                cardStates.Add(
                    new CardStateJson(
                        card.ToSerializable(),
                        new PositionJson(self, new() { "bench" })
                    )
                );
                AddAttachedCards(card as IPokemonCard, self, cardStates);
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
                        card.ToSerializable(),
                        new PositionJson(owner, attachedToPokemonId: pokemonCard.DeckId)
                    )
                );
            }
            foreach (var card in pokemonCard.PreEvolutions)
            {
                cardStates.Add(
                    new CardStateJson(
                        card.ToSerializable(),
                        new PositionJson(owner, attachedToPokemonId: pokemonCard.DeckId)
                    )
                );
            }
        }

        private static void AddDiscardPileAs(
            IPlayer player,
            Owner owner,
            List<CardStateJson> cardStates
        )
        {
            foreach (var card in player.DiscardPile.Cards)
            {
                cardStates.Add(
                    new CardStateJson(
                        card.ToSerializable(),
                        new PositionJson(owner, new() { "discard_pile" })
                    )
                );
            }
        }

        private static void AddPrizesAsSelf(IPlayerLogic self, List<CardStateJson> cardStates)
        {
            foreach (var card in self.Prizes.Cards)
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

        private static void AddPrizesAsOpponent(
            IPlayerLogic opponent,
            List<CardStateJson> cardStates
        )
        {
            foreach (var card in opponent.Prizes.Cards)
            {
                if (card.OpponentPositionKnowledge == PositionKnowledge.Unknown)
                {
                    cardStates.Add(
                        new CardStateJson(
                            CardJson.CreateUnknownCard(),
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

        private static void AddHandAsSelf(IPlayerLogic self, List<CardStateJson> cardStates)
        {
            foreach (var card in self.Hand.Cards)
            {
                cardStates.Add(
                    new CardStateJson(
                        card.ToSerializable(),
                        new PositionJson(Owner.Self, new() { "hand" })
                    )
                );
            }
        }

        private static void AddHandAsOpponent(IPlayerLogic opponent, List<CardStateJson> cardStates)
        {
            foreach (var card in opponent.Hand.Cards)
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
                                CardJson.CreateUnknownCard(),
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

        private static void AddDeckAsSelf(IPlayerLogic self, List<CardStateJson> cardStates)
        {
            foreach (var card in self.Deck.Cards)
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

        private static void AddDeckAsOpponent(IPlayerLogic opponent, List<CardStateJson> cardStates)
        {
            foreach (var card in opponent.Deck.Cards)
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
                                CardJson.CreateUnknownCard(),
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
