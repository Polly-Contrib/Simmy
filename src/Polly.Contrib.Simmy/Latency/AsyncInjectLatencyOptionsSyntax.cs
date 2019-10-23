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
        /// Builds a <see cref="AsyncInjectLatencyPolicy"/> which injects latency if <paramref name="configureOptions.Enabled"/> returns true and
        /// a random number is within range of <paramref name="configureOptions.InjectionRate"/>.
        /// </summary>
        /// <param name="configureOptions">A callback to configure policy options.</param>
        /// <returns>The policy instance.</returns>
        public static AsyncInjectLatencyPolicy InjectLatencyAsync(Action<InjectLatencyAsyncOptions> configureOptions)
        {
            var options = new InjectLatencyAsyncOptions();
            configureOptions.Invoke(options);

            if (options.Latency == null) throw new ArgumentNullException(nameof(options.Latency));
            if (options.InjectionRate == null) throw new ArgumentNullException(nameof(options.InjectionRate));
            if (options.Enabled == null) throw new ArgumentNullException(nameof(options.Enabled));

            return new AsyncInjectLatencyPolicy(options);
        }

        /// <summary>
        /// Builds a <see cref="AsyncInjectLatencyPolicy"/> which injects latency if <paramref name="configureOptions.Enabled"/> returns true and
        /// a random number is within range of <paramref name="configureOptions.InjectionRate"/>.
        /// </summary>
        /// <param name="configureOptions">A callback to configure policy options.</param>
        /// <returns>The policy instance.</returns>
        public static AsyncInjectLatencyPolicy<TResult> InjectLatencyAsync<TResult>(Action<InjectLatencyAsyncOptions> configureOptions)
        {
            var options = new InjectLatencyAsyncOptions();
            configureOptions.Invoke(options);

            if (options.Latency == null) throw new ArgumentNullException(nameof(options.Latency));
            if (options.InjectionRate == null) throw new ArgumentNullException(nameof(options.InjectionRate));
            if (options.Enabled == null) throw new ArgumentNullException(nameof(options.Enabled));

            return new AsyncInjectLatencyPolicy<TResult>(options);
        }
    }
}
