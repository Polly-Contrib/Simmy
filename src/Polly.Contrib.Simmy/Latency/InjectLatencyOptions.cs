using System;
using System.Threading;
using Polly.Contrib.Simmy.Latency;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Options used to configure an <see cref="AsyncInjectLatencyPolicy"/>
    /// </summary>
    public class InjectLatencyOptions : InjectOptionsBase
    {
        /// <summary>
        /// Latency Delegate to be executed
        /// </summary>
        internal Func<Context, CancellationToken, TimeSpan> Latency { get; set; }
    }
}
