using System;
using System.Threading;

namespace Polly.Contrib.Simmy.Latency
{
    /// <summary>
    /// Options used to configure an <see cref="AsyncInjectLatencyPolicy"/>
    /// </summary>
    public class InjectLatencyOptions : InjectOptionsBase
    {
        /// <summary>
        /// Latency Delegate to be executed
        /// </summary>
        internal Func<Context, CancellationToken, TimeSpan> LatencyInternal { get; set; }
    }
}
