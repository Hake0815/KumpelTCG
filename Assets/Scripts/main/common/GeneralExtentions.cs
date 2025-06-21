using System;

namespace gamecore.common
{
    public static class Extensions
    {
        public static void Let<T>(this T obj, Action<T> action)
        {
            if (obj != null)
                action(obj);
        }
    }
}
