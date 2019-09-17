using System;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Options used to configure a <see cref="MonkeyPolicy"/>
    /// </summary>
    public class DefaultOptions
    {
        /// <summary>
        /// Lambda to check if this policy is enabled in context free mode
        /// </summary>
        public Func<bool> Enabled { get; set; }

        /// <summary>
        /// The injection rate between [0, 1]
        /// </summary>
        public double InjectionRate { get; set; }
    }
}
