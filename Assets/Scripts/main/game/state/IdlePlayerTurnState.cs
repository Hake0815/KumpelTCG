using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamecore.card;

namespace gamecore.game.state
{
    internal class IdlePlayerTurnState : IGameState
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
            if (!player.IsActive)
                return new();

            var interactions = new List<GameInteraction>();
            AddPlayCardInteractions(interactions, gameController, player);
            AddPlayCardWithTargetsInteractions(interactions, gameController, player);
            if (gameController.Game.TurnCounter > 1)
                AddAttackInteractions(interactions, gameController, player);
            interactions.Add(
                new GameInteraction(gameController.EndTurn, GameInteractionType.EndTurn)
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
                        () => gameController.PlayCard(card),
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
                var targets = card.GetTargets();
                interactions.Add(
                    new GameInteraction(
                        (selectedTargets) =>
                            gameController.PlayCardWithTargets(
                                card,
                                selectedTargets.Cast<ICardLogic>().ToList()
                            ),
                        GameInteractionType.PlayCardWithTargets,
                        new()
                        {
                            new InteractionCard(card),
                            new TargetData(
                                card.GetTargets().Count,
                                targets.Cast<object>().ToList()
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
            foreach (var attack in player.ActivePokemon.GetUsableAttacks())
            {
                interactions.Add(
                    new GameInteraction(
                        () => gameController.PerformAttack(attack, player.ActivePokemon),
                        GameInteractionType.PerformAttack,
                        new() { new InteractionCard(player.ActivePokemon), new AttackData(attack) }
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
