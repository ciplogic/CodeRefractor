#region Uses

using System;

#endregion

namespace CodeRefractor.RuntimeBase
{
    public static class Ensure
    {
        public static void AreEqual<T>(T expected, T actual, string message)
        {
            if (!expected.Equals(actual))
                throw new InvalidOperationException(message);
        }

        public static void IsTrue(bool value, string message)
        {
            if (!value)
                throw new InvalidOperationException(message);
        }
    }
}