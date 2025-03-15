using System.Collections.Generic;

public class Game
{
    public IPlayer Player1 { get; private set; }

    public IPlayer Player2 { get; private set; }

    public Game()
    {
        Player1 = new Player(new Deck());
        Player2 = new Player(new Deck());
    }

    public Game(IPlayer player1, IPlayer player2)
    {
        Player1 = player1;
        Player2 = player2;
    }

    public void SetUp(List<ICard> cardsPlayer1, List<ICard> cardsPlayer2)
    {
        Player1.Deck.SetUp(cardsPlayer1);
        Player2.Deck.SetUp(cardsPlayer2);
    }

    public void StartGame()
    {
        StartTurn(Player1);
    }

    public void EndTurn()
    {
        if (Player1.IsActive)
        {
            Player1.IsActive = false;
            StartTurn(Player2);
        }
        else
        {
            Player2.IsActive = false;
            StartTurn(Player1);
        }
    }

    private void StartTurn(IPlayer player)
    {
        player.IsActive = true;
        player.Draw();
    }
}
