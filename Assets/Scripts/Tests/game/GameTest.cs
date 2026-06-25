using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using gamecore.serialization;
using Moq;
using NUnit.Framework;

namespace gamecore.game
{
    public class GameTest
    {
        private Mock<IPlayerLogic> player1;
        private Mock<IPlayerLogic> player2;
        private Game game;

        [SetUp]
        public void SetUp()
        {
            player1 = new();
            player2 = new();
            game = new Game(
                player1.Object,
                player2.Object,
                new ActionSystem(Path.GetTempFileName())
            );
        }

        [Test]
        public async Task ShouldStartTurnForPlayer2()
        {
            player1.SetupGet(p => p.IsActive).Returns(true);

            var endTurnGA = await game.Perform(new EndTurnGA());

            player2.VerifySet(p => p.IsActive = true);
            Assert.AreSame(endTurnGA.NextPlayer, player2.Object);
        }

        [Test]
        public void ShouldSetPlayer1Inactive()
        {
            player1.SetupGet(p => p.IsActive).Returns(true);

            game.Perform(new EndTurnGA());

            player1.VerifySet(p => p.IsActive = false);
        }

        [Test]
        public async Task InitializationShouldHideOpponentStaticAndDynamicData()
        {
            var controller = await CreateControllerWithGame();

            var initialization = controller.ExportGameInitialization("Player 1");

            Assert.That(initialization.CardStatics, Has.Count.EqualTo(120));
            Assert.That(
                initialization.CardStatics.Where(card => card.DeckId < 60),
                Has.All.Matches<ProtoBufCardStatic>(
                    card => card.CardType != ProtoBufCardType.CardTypeUnknown
                )
            );
            Assert.That(
                initialization.CardStatics.Where(card => card.DeckId >= 60),
                Has.All.Matches<ProtoBufCardStatic>(
                    card => card.CardType == ProtoBufCardType.CardTypeUnknown
                )
            );
            Assert.That(
                initialization
                    .InitialState.CardStates.Where(state =>
                        state.Position.Owner == ProtoBufOwner.OwnerOpponent
                    )
                    .Select(state => state.CardDynamic),
                Has.All.Null
            );
            var staticsByDeckId = initialization.CardStatics.ToDictionary(card => card.DeckId);
            foreach (
                var state in initialization.InitialState.CardStates.Where(state =>
                    state.Position.Owner == ProtoBufOwner.OwnerSelf
                )
            )
            {
                var cardType = staticsByDeckId[state.DeckId].CardType;
                if (cardType == ProtoBufCardType.CardTypeTrainer)
                {
                    Assert.That(state.CardDynamic, Is.Null);
                }
                else
                {
                    Assert.That(state.CardDynamic, Is.Not.Null);
                }
            }
        }

        [Test]
        public async Task StateShouldUpsertStaticDataWhenOpponentCardBecomesKnown()
        {
            var controller = await CreateControllerWithGame();
            controller.ExportGameInitialization("Player 1");
            var opponentCard = ((Game)controller.Game)
                .Player2.DeckList.Cards.First(card =>
                    card.OpponentPositionKnowledge == PositionKnowledge.Unknown
                );
            opponentCard.OpponentPositionKnowledge = PositionKnowledge.Known;

            var firstUpdate = controller.ExportGameState("Player 1");
            var secondUpdate = controller.ExportGameState("Player 1");

            Assert.That(firstUpdate.CardStaticUpserts, Has.Count.EqualTo(1));
            Assert.That(firstUpdate.CardStaticUpserts[0].DeckId, Is.EqualTo(opponentCard.DeckId));
            Assert.That(
                firstUpdate.CardStaticUpserts[0].CardType,
                Is.Not.EqualTo(ProtoBufCardType.CardTypeUnknown)
            );
            Assert.That(secondUpdate.CardStaticUpserts, Is.Empty);
            Assert.That(
                controller
                    .ExportGameInitialization("Player 1")
                    .CardStatics.Single(card => card.DeckId == opponentCard.DeckId)
                    .CardType,
                Is.Not.EqualTo(ProtoBufCardType.CardTypeUnknown)
            );
        }

        [Test]
        public async Task InitializationSnapshotShouldRecreateGame()
        {
            var controller = await CreateControllerWithGame();
            var initialization = controller.ExportGameInitialization("Player 1");
            var recreatedController = new GameController(Path.GetTempFileName());
            var deckList = CreateDeckList();

            recreatedController.RecreateGameFromGameState(
                initialization,
                deckList,
                deckList,
                "Player 1",
                "Player 2"
            );
            var recreatedInitialization = recreatedController.ExportGameInitialization("Player 1");

            Assert.That(recreatedInitialization.CardStatics, Is.EqualTo(initialization.CardStatics));
            Assert.That(
                recreatedInitialization.InitialState.CardStates,
                Has.Count.EqualTo(initialization.InitialState.CardStates.Count)
            );
        }

        private static async Task<GameController> CreateControllerWithGame()
        {
            var controller = new GameController(Path.GetTempFileName());
            var deckList = CreateDeckList();
            await controller.CreateGame(deckList, deckList, "Player 1", "Player 2");
            return controller;
        }

        private static Dictionary<string, int> CreateDeckList()
        {
            return new Dictionary<string, int>
            {
                { "TWM128", 20 },
                { "professorsResearch", 20 },
                { "FireNRG", 20 },
            };
        }
    }
}
