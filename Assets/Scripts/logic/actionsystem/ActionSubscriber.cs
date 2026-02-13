namespace gamecore.actionsystem
{
    public interface IActionSubscriber<T>
        where T : GameAction
    {
        T React(T action);
    }
}
