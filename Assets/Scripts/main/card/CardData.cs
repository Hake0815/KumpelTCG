using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using gamecore.instruction;

namespace gamecore.card
{
    internal interface ICardData
    {
        string Name { get; }
        string Id { get; }
    }
}
