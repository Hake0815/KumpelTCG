using System;

namespace gamecore.common
{
    public interface IClonable<T>
    {
        T Clone();
    }
}
