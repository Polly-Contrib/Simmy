using System;
using Polly.Contrib.Simmy.Latency;
using Polly.Contrib.Simmy.Latency.Options;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Fluent API for defining Monkey <see cref="Policy"/>. 
    /// </summary>
    public partial class MonkeyPolicy
    {
        /// <summary>
        /// Builds a <see cref="InjectLatencyPolicy"/> which injects latency if <paramref name="configureOptions.Enabled"/> returns true and
        /// a random number is within range of <paramref name="configureOptions.InjectionRate"/>.
        /// </summary>
        /// <param name="configureOptions">A callback to configure policy options.</param>
        /// <returns>The policy instance.</returns>
        public static InjectLatencyPolicy InjectLatency(Action<InjectLatencyOptions> configureOptions)
        {
            var options = new InjectLatencyOptions();
            configureOptions.Invoke(options);

            if (options.Latency == null) throw new ArgumentNullException(nameof(options.Latency));
            if (options.InjectionRate == null) throw new ArgumentNullException(nameof(options.InjectionRate));
            if (options.Enabled == null) throw new ArgumentNullException(nameof(options.Enabled));

            return new InjectLatencyPolicy(options);
        }

        /// <summary>
        /// Builds a <see cref="AsyncInjectLatencyPolicy"/> which injects latency if <paramref name="configureOptions.Enabled"/> returns true and
        /// a random number is within range of <paramref name="configureOptions.InjectionRate"/>.
        /// </summary>
        /// <param name="configureOptions">A callback to configure policy options.</param>
        /// <returns>The policy instance.</returns>
        public static InjectLatencyPolicy<TResult> InjectLatency<TResult>(Action<InjectLatencyOptions> configureOptions)
        {
            var options = new InjectLatencyOptions();
            configureOptions.Invoke(options);

            if (options.Latency == null) throw new ArgumentNullException(nameof(options.Latency));
            if (options.InjectionRate == null) throw new ArgumentNullException(nameof(options.InjectionRate));
            if (options.Enabled == null) throw new ArgumentNullException(nameof(options.Enabled));

            return new InjectLatencyPolicy<TResult>(options);
        }
    }
}
