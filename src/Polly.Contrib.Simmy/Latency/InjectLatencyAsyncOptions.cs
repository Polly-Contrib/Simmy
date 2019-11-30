using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Contrib.Simmy.Latency
{
    /// <summary>
    /// Options used to configure an <see cref="AsyncInjectLatencyPolicy"/>
    /// </summary>
    public class InjectLatencyAsyncOptions : InjectOptionsAsyncBase
    {
        /// <summary>
        /// Latency Delegate to be executed
        /// </summary>
        internal Func<Context, CancellationToken, Task<TimeSpan>> LatencyInternal { get; set; }
    }
}
