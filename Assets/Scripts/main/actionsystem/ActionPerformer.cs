using System.Threading.Tasks;

namespace gamecore.actionsystem
{
    public interface IActionPerformer<T>
        where T : GameAction
    {
        Task<T> Perform(T action);
    }
}
