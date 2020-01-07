using System;

namespace Polly.Contrib.Simmy.Utilities
{
    internal static class GuardExtensions
    {
        public static void EnsureInjectionThreshold(this double injectionThreshold)
        {
            if (injectionThreshold < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(injectionThreshold), "Injection rate/threshold in Monkey policies should always be a double between [0, 1]; never a negative number.");
            }
            if (injectionThreshold > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(injectionThreshold), "Injection rate/threshold in Monkey policies should always be a double between [0, 1]; never a number greater than 1.");
            }
        }
    }
}
