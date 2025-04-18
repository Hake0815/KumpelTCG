using gamecore.actionsystem;

namespace gamecore.game.action
{
    internal class DrawCardGA : GameAction
    {
        public int Amount { get; }
        public IPlayer Player { get; }

        public DrawCardGA(int amount, IPlayer player)
        {
            Amount = amount;
            Player = player;
        }
    }
}
