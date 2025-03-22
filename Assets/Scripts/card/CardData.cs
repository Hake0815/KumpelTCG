using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using gamecore.effect;

namespace gamecore.card
{
    public interface ICardData
    {
        public string Name { get; }
        public string Id { get; }
    }
}
