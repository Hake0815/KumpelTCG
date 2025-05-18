using System.Threading.Tasks;
using gamecore.card;

namespace gamecore.effect
{
    internal interface IEffect
    {
        void Perform(ICardLogic card);
    }
}
