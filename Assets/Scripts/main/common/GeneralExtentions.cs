using System;

namespace gamecore.common
{
    public static class Extensions
    {
        public static void Let<T>(this T obj, Action<T> action)
        {
            if (!object.Equals(obj, default(T)))
                action(obj);
        }

        public static void Let<T>(this T obj, Action<T> action, Action elseAction)
        {
            if (!object.Equals(obj, default(T)))
                action(obj);
            else
                elseAction.Invoke();
        }
    }
}
