using gamecore.game.action;
using Moq;
using NUnit.Framework;

namespace gamecore.game
{
    public class GameTest
    {
        private Mock<IPlayer> player1;
        private Mock<IPlayer> player2;
        private Game game;

        [SetUp]
        public void SetUp()
        {
            player1 = new();
            player2 = new();
            game = new Game(player1.Object, player2.Object);
        }

        [Test]
        public void ShouldStartTurnForPlayer1()
        {
            game.StartGame();

            player1.VerifySet(p => p.IsActive = true);
            player1.Verify(p => p.Draw());
        }

        [Test]
        public void ShouldStartTurnForPlayer2()
        {
            player1.SetupGet(p => p.IsActive).Returns(true);

            var endTurnGA = game.EndTurn(new EndTurnGA());

            player2.VerifySet(p => p.IsActive = true);
            Assert.AreSame(endTurnGA.NextPlayer, player2.Object);
        }

        [Test]
        public void ShouldSetPlayer1Inactive()
        {
            player1.SetupGet(p => p.IsActive).Returns(true);

            game.EndTurn(new EndTurnGA());

            player1.VerifySet(p => p.IsActive = false);
        }
    }
}
