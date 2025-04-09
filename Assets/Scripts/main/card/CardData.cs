using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using gamecore.effect;

namespace gamecore.card
{
    public interface ICardData
    {
        string Name { get; }
        string Id { get; }
    }
}
