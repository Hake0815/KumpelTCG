using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamecore.card;
using UnityEngine;

namespace gamecore.game.state
{
    class IdlePlayerTurnState : IGameState
    {
        public IGameState AdvanceSuccesfully()
        {
            return this;
        }

        public List<GameInteraction> GetGameInteractions(
            GameController gameController,
            IPlayerLogic player
        )
        {
            Debug.Log(
                $"Get interactions for player {player.Name} who is active: {player.IsActive}"
            );
            if (!player.IsActive)
                return new();

            var interactions = new List<GameInteraction>();
            AddPlayCardInteractions(interactions, gameController, player);
            AddPlayCardWithTargetsInteractions(interactions, gameController, player);
            AddAttackInteractions(interactions, gameController, player);
            AddRetreatInteraction(interactions, gameController, player);
            AddAbilityInteraction(interactions, gameController, player);
            interactions.Add(
                new GameInteraction(
                    async () => await gameController.EndTurn(),
                    GameInteractionType.EndTurn
                )
            );
            return interactions;
        }

        private static void AddPlayCardInteractions(
            List<GameInteraction> interactions,
            GameController gameController,
            IPlayerLogic player
        )
        {
            var playableCards = GetPlayableCardsFromHand(player);
            foreach (var card in playableCards)
            {
                interactions.Add(
                    new GameInteraction(
                        async () => await gameController.PlayCard(card),
                        GameInteractionType.PlayCard,
                        new() { new InteractionCard(card) }
                    )
                );
            }
        }

        private static List<ICardLogic> GetPlayableCardsFromHand(IPlayerLogic player)
        {
            var playableCards = new List<ICardLogic>();
            foreach (var card in player.Hand.Cards)
            {
                if (card.IsPlayable())
                    playableCards.Add(card);
            }
            return playableCards;
        }

        private static void AddPlayCardWithTargetsInteractions(
            List<GameInteraction> interactions,
            GameController gameController,
            IPlayerLogic player
        )
        {
            var playableCards = GetPlayableCardsWithTargetFromHand(player);
            foreach (var card in playableCards)
            {
                var targets = card.GetPossibleTargets();
                interactions.Add(
                    new GameInteraction(
                        async (selectedTargets) =>
                            await gameController.PlayCardWithTargets(
                                card,
                                selectedTargets.Cast<ICardLogic>().ToList()
                            ),
                        GameInteractionType.PlayCardWithTargets,
                        new()
                        {
                            new InteractionCard(card),
                            new TargetData(
                                card.GetNumberOfTargets(),
                                targets.Cast<ICard>().ToList()
                            ),
                        }
                    )
                );
            }
        }

        private static List<ICardLogic> GetPlayableCardsWithTargetFromHand(IPlayerLogic player)
        {
            var playableCards = new List<ICardLogic>();
            foreach (var card in player.Hand.Cards)
            {
                if (card.IsPlayableWithTargets())
                    playableCards.Add(card);
            }
            return playableCards;
        }

        private static void AddAttackInteractions(
            List<GameInteraction> interactions,
            GameController gameController,
            IPlayerLogic player
        )
        {
            if (gameController.Game.TurnCounter < 2)
                return;
            foreach (var attack in player.ActivePokemon.GetUsableAttacks())
            {
                interactions.Add(
                    new GameInteraction(
                        async () =>
                            await gameController.PerformAttack(attack, player.ActivePokemon),
                        GameInteractionType.PerformAttack,
                        new() { new InteractionCard(player.ActivePokemon), new AttackData(attack) }
                    )
                );
            }
        }

        private static void AddRetreatInteraction(
            List<GameInteraction> interactions,
            GameController gameController,
            IPlayerLogic player
        )
        {
            if (
                !player.ActivePokemon.CanPayRetreatCost()
                || player.PerformedOncePerTurnActions.Contains(PokemonCard.RETREATED)
                || player.Bench.CardCount == 0
            )
                return;

            var pokemon = player.ActivePokemon;
            if (IsRetreatCostUnambiguous(player.ActivePokemon))
            {
                AddRetreatInteractionWithUnambiguousCost(
                    interactions,
                    gameController,
                    player,
                    pokemon
                );
            }
            else
            {
                AddRetreatInteractionWithInput(interactions, gameController, player, pokemon);
            }
        }

        private static void AddRetreatInteractionWithInput(
            List<GameInteraction> interactions,
            GameController gameController,
            IPlayerLogic player,
            IPokemonCardLogic pokemon
        )
        {
            interactions.Add(
                new GameInteraction(
                    async (energyCardsToDiscard) =>
                        await gameController.Retreat(
                            player.ActivePokemon,
                            energyCardsToDiscard.Cast<IEnergyCardLogic>().ToList()
                        ),
                    GameInteractionType.Retreat,
                    new()
                    {
                        new InteractionCard(player.ActivePokemon),
                        new ConditionalTargetData(
                            selection => FulfillsRetreatCost(selection, pokemon.RetreatCost),
                            pokemon.AttachedEnergyCards.Cast<ICard>().ToList()
                        ),
                    }
                )
            );
        }

        private static void AddRetreatInteractionWithUnambiguousCost(
            List<GameInteraction> interactions,
            GameController gameController,
            IPlayerLogic player,
            IPokemonCardLogic pokemon
        )
        {
            var energyCardsToDiscard = new List<IEnergyCardLogic>();
            int payedEnergyCost = 0;
            foreach (var energy in pokemon.AttachedEnergyCards)
            {
                energyCardsToDiscard.Add(energy);
                payedEnergyCost++; // Needs to change as soon as energy cards are implemented that provide more than one energy
                if (payedEnergyCost >= pokemon.RetreatCost)
                    break;
            }
            interactions.Add(
                new GameInteraction(
                    async () =>
                        await gameController.Retreat(player.ActivePokemon, energyCardsToDiscard),
                    GameInteractionType.Retreat,
                    new() { new InteractionCard(player.ActivePokemon) }
                )
            );
        }

        private static bool FulfillsRetreatCost(List<ICard> selectedEnergyCards, int retreatCost)
        {
            return selectedEnergyCards.Count >= retreatCost;
        }

        private static bool IsRetreatCostUnambiguous(IPokemonCardLogic pokemon)
        {
            return pokemon.AttachedEnergy.Count == pokemon.RetreatCost
                || (
                    ContainsSameCards(pokemon.AttachedEnergyCards)
                    && pokemon.AttachedEnergy.Count == pokemon.AttachedEnergyCards.Count
                );
        }

        private static bool ContainsSameCards(List<IEnergyCardLogic> attachedEnergyCards)
        {
            if (attachedEnergyCards.Count <= 1)
                return true;
            var firstEnergyCardId = attachedEnergyCards[0].Id;
            for (int i = 1; i < attachedEnergyCards.Count; i++)
            {
                if (attachedEnergyCards[i].Id != firstEnergyCardId)
                    return false;
            }
            return true;
        }

        private static void AddAbilityInteraction(
            List<GameInteraction> interactions,
            GameController gameController,
            IPlayerLogic player
        )
        {
            AddAbilityIfPossible(player.ActivePokemon, interactions, gameController);
            foreach (var pokemon in player.Bench.Cards)
            {
                AddAbilityIfPossible(pokemon as IPokemonCardLogic, interactions, gameController);
            }
        }

        private static void AddAbilityIfPossible(
            IPokemonCardLogic pokemon,
            List<GameInteraction> interactions,
            GameController gameController
        )
        {
            if (pokemon.HasUsableAbility())
            {
                interactions.Add(
                    new GameInteraction(
                        async () => await gameController.PerformAbility(pokemon),
                        GameInteractionType.PerformAbility,
                        new() { new InteractionCard(pokemon) }
                    )
                );
            }
        }

        public Task OnAdvanced(Game game)
        {
            game.AwaitInteraction();
            return Task.CompletedTask;
        }
    }
}
