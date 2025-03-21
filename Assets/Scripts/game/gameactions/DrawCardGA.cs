using gamecore.actionsystem;

namespace gamecore.game.action
{
    public class DrawCardGA : GameAction
    {
        public DrawCardGA(int amount, IPlayer player)
        {
            Amount = amount;
            Player = player;
        }

        public int Amount { get; set; }
        public IPlayer Player { get; set; }
    }
}
