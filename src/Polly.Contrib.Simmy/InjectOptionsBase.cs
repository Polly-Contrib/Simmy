using System;
using System.Threading;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Options used to configure a <see cref="MonkeyPolicy"/>
    /// </summary>
    public abstract class InjectOptionsBase
    {
        /// <summary>
        /// Lambda to get injection rate between [0, 1]
        /// </summary>
        internal Func<Context, CancellationToken, Double> InjectionRate { get; set; }

        /// <summary>
        /// Lambda to check if this policy is enabled in current context
        /// </summary>
        internal Func<Context, CancellationToken, bool> Enabled { get; set; }

        /// <summary>
        /// Lambda to call immediately before injecting
        /// </summary>
        internal Action<Context, CancellationToken> BeforeInjectCallback { get; set; }
    }
}
