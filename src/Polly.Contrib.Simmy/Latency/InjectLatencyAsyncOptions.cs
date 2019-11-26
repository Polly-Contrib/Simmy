using System;
using System.Threading;
using System.Threading.Tasks;
using Polly.Contrib.Simmy.Latency;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Options used to configure an <see cref="AsyncInjectLatencyPolicy"/>
    /// </summary>
    public class InjectLatencyAsyncOptions : InjectOptionsAsyncBase
    {
        /// <summary>
        /// Latency Delegate to be executed
        /// </summary>
        internal Func<Context, CancellationToken, Task<TimeSpan>> Latency { get; set; }
    }
}
