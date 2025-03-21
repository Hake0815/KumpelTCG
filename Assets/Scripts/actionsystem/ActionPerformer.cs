namespace gamecore.actionsystem
{
    public interface IActionPerformer<T>
        where T : GameAction
    {
        T Perform(T action);
    }
}
