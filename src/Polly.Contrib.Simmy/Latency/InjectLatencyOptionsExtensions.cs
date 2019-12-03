using System;
using System.Threading;

namespace Polly.Contrib.Simmy.Latency
{
    /// <summary>
    /// Allows configuration of behaviour for asynchronous monkey behaviour-injection policies.
    /// </summary>
    public static class InjectLatencyOptionsExtensions
    {
        /// <summary>
        /// Configure behaviour to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="latency">The latency to inject.</param>
        public static InjectLatencyOptions Latency(this InjectLatencyOptions options, TimeSpan latency) =>
            Latency(options, (_, __) => latency);

        /// <summary>
        /// Configure behaviour to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="latency">A delegate representing the latency to inject.</param>
        public static InjectLatencyOptions Latency(this InjectLatencyOptions options, Func<Context, CancellationToken, TimeSpan> latency)
        {
            options.LatencyInternal = latency;
            return options;
        }
    }
}
