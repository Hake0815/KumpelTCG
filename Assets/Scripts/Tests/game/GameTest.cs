using System.Threading.Tasks;
using gamecore.game.action;
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
            game = new Game(player1.Object, player2.Object);
        }

        [Test]
        public async Task ShouldStartTurnForPlayer1()
        {
            await game.StartGame();

            player1.VerifySet(p => p.IsActive = true);
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
    }
}
