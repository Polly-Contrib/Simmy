using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Contrib.Simmy.Latency
{
    /// <summary>
    /// Allows configuration of behaviour for asynchronous monkey behaviour-injection policies.
    /// </summary>
    public static class InjectLatencyAsyncOptionsExtensions
    {
        /// <summary>
        /// Configure behaviour to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="latency">The latency to inject.</param>
        public static InjectLatencyAsyncOptions Latency(this InjectLatencyAsyncOptions options, TimeSpan latency) =>
            Latency(options, (_, __) => Task.FromResult(latency));

        /// <summary>
        /// Configure behaviour to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="latency">A delegate representing the latency to inject.</param>
        public static InjectLatencyAsyncOptions Latency(this InjectLatencyAsyncOptions options, Func<Context, CancellationToken, Task<TimeSpan>> latency)
        {
            options.Latency = latency;
            return options;
        }
    }
}
