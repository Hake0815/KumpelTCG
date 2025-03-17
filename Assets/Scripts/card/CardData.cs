namespace gamecore.card
{
    public abstract class CardData
    {
        public string Name { get; private set; }

        protected CardData(string name)
        {
            Name = name;
        }
    }

    public class CardDataDummy : CardData
    {
        public CardDataDummy(string name)
            : base(name) { }
    }
}
