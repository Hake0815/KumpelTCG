namespace gamecore.instruction.filter
{
    public readonly struct IntRange
    {
        public int Min { get; }
        public int Max { get; }

        public IntRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public bool Contains(int value) => value >= Min && value <= Max;
    }
}
