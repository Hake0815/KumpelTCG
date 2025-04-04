using System.Collections;
using System.Collections.Generic;
using gamecore.actionsystem;
using gamecore.card;
using gamecore.game.action;
using NUnit.Framework.Constraints;

namespace gamecore.game
{
    public interface IGame
    {
        public IPlayer Player1 { get; }
        public IPlayer Player2 { get; }
    }

    internal class Game : IGame, IActionPerformer<EndTurnGA>
    {
        internal IPlayerLogic Player1 { get; private set; }
        internal IPlayerLogic Player2 { get; private set; }
        IPlayer IGame.Player1
        {
            get => Player1;
        }
        IPlayer IGame.Player2
        {
            get => Player2;
        }

        internal GameSetupBuilder GameSetupBuilder { get; private set; }

        internal Game()
        {
            Player1 = new Player(new Deck());
            Player1.Name = "Player1";
            Player2 = new Player(new Deck());
            Player2.Name = "Player2";
        }

        internal Game(IPlayerLogic player1, IPlayerLogic player2)
        {
            Player1 = player1;
            Player2 = player2;
        }

        internal void Initialize(List<ICard> cardsPlayer1, List<ICard> cardsPlayer2)
        {
            ActionSystem.INSTANCE.AttachPerformer<EndTurnGA>(this);
            CardSystem.INSTANCE.Enable();
            Player1.Deck.SetUp(cardsPlayer1);
            Player2.Deck.SetUp(cardsPlayer2);
        }

        internal void PerformSetup()
        {
            GameSetupBuilder = new GameSetupBuilder().WithPlayer1(Player1).WithPlayer2(Player2);
            GameSetupBuilder.Setup();
        }

        internal void StartGame()
        {
            Player1.IsActive = true;
            ActionSystem.INSTANCE.Perform(new DrawCardGA(1, Player1));
        }

        internal EndTurnGA Perform(EndTurnGA action)
        {
            return EndTurn(action);
        }

        internal EndTurnGA EndTurn(EndTurnGA endTurnGA)
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

        internal void EndGame()
        {
            ActionSystem.INSTANCE.DetachPerformer<EndTurnGA>();
            CardSystem.INSTANCE.Disable();
        }
    }
}
