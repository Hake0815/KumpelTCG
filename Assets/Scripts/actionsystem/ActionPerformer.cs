namespace gamecore.actionsystem
{
    internal interface IActionPerformer<T>
        where T : GameAction
    {
        T Perform(T action);
    }
}
