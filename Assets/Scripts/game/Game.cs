using System.Collections;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using NUnit.Framework.Constraints;

namespace gamecore.game
{
    public class Game : IActionPerformer<EndTurnGA>
    {
        public IPlayer Player1 { get; private set; }
        public IPlayer Player2 { get; private set; }
        public GameSetupBuilder GameSetupBuilder { get; private set; }

        public Game()
        {
            Player1 = new Player(new Deck()) { Name = "Player 1" };
            Player2 = new Player(new Deck()) { Name = "Player 2" };
        }

        public Game(IPlayer player1, IPlayer player2)
        {
            Player1 = player1;
            Player2 = player2;
        }

        public void Initialize(List<ICard> cardsPlayer1, List<ICard> cardsPlayer2)
        {
            ActionSystem.INSTANCE.AttachPerformer<EndTurnGA>(this);
            CardSystem.INSTANCE.Enable();
            Player1.Deck.SetUp(cardsPlayer1);
            Player2.Deck.SetUp(cardsPlayer2);
        }

        public void PerformSetup()
        {
            GameSetupBuilder = new GameSetupBuilder().WithPlayer1(Player1).WithPlayer2(Player2);
            GameSetupBuilder.Setup();
        }

        public void StartGame()
        {
            Player1.IsActive = true;
            ActionSystem.INSTANCE.Perform(new DrawCardGA(1, Player1));
        }

        public EndTurnGA Perform(EndTurnGA action)
        {
            return EndTurn(action);
        }

        public EndTurnGA EndTurn(EndTurnGA endTurnGA)
        {
            if (Player1.IsActive)
            {
                Player1.IsActive = false;
                Player2.IsActive = true;
                endTurnGA.NextPlayer = Player2;
            }
            else
            {
                Player2.IsActive = false;
                Player1.IsActive = true;
                endTurnGA.NextPlayer = Player1;
            }
            return endTurnGA;
        }

        public void EndGame()
        {
            ActionSystem.INSTANCE.DetachPerformer<EndTurnGA>();
            CardSystem.INSTANCE.Disable();
        }
    }
}
