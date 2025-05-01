using System.Threading.Tasks;
using gamecore.card;

namespace gamecore.effect
{
    internal interface IEffect
    {
        Task Perform(ICardLogic card);
    }
}
