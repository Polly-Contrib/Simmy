using System;
using System.Threading;
using Polly.Contrib.Simmy.Latency;

namespace Polly.Contrib.Simmy
{
    public partial class MonkeyPolicy
    {
        /// <summary>
        /// Builds an <see cref="InjectLatencyPolicy"/> which injects latency if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="latency">The latency to inject</param>
        /// <param name="injectionRate">injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in context free fashion</param>
        /// <returns>The policy instance.</returns>
        [Obsolete("This overload is going to be deprecated, use the overload which takes Action<InjectLatencyOptions> instead.")]
        public static InjectLatencyPolicy InjectLatency(
            TimeSpan latency,
            Double injectionRate,
            Func<bool> enabled)
        {
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            return new InjectLatencyPolicy((_, __) => latency, (_, __) => injectionRate, (_, __) => enabled());
        }

        /// <summary>
        /// Builds an <see cref="InjectLatencyPolicy"/> which injects latency if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="latency">The latency to inject</param>
        /// <param name="injectionRate">injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in current context</param>
        /// <returns>The policy instance.</returns>
        [Obsolete("This overload is going to be deprecated, use the overload which takes Action<InjectLatencyOptions> instead.")]
        public static InjectLatencyPolicy InjectLatency(
            TimeSpan latency,
            Double injectionRate,
            Func<Context, CancellationToken, bool> enabled)
        {
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            return new InjectLatencyPolicy((_, __) => latency, (_, __) => injectionRate, enabled);
        }

        /// <summary>
        /// Builds an <see cref="InjectLatencyPolicy"/> which injects latency if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="latency">lambda to get the latency object</param>
        /// <param name="injectionRate">lambda to get injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in current context</param>
        /// <returns>The policy instance.</returns>
        [Obsolete("This overload is going to be deprecated, use the overload which takes Action<InjectLatencyOptions> instead.")]
        public static InjectLatencyPolicy InjectLatency(
            TimeSpan latency,
            Func<Context, CancellationToken, Double> injectionRate,
            Func<Context, CancellationToken, bool> enabled)
        {
            if (injectionRate == null) throw new ArgumentNullException(nameof(injectionRate));
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            return new InjectLatencyPolicy((_, __) => latency, injectionRate, enabled);
        }

        /// <summary>
        /// Builds an <see cref="InjectLatencyPolicy"/> which injects latency if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="latency">lambda to get the latency object</param>
        /// <param name="injectionRate">lambda to get injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in current context</param>
        /// <returns>The policy instance.</returns>
        [Obsolete("This overload is going to be deprecated, use the overload which takes Action<InjectLatencyOptions> instead.")]
        public static InjectLatencyPolicy InjectLatency(
            Func<Context, CancellationToken, TimeSpan> latency,
            Func<Context, CancellationToken, Double> injectionRate,
            Func<Context, CancellationToken, bool> enabled)
        {
            if (latency == null) throw new ArgumentNullException(nameof(latency));
            if (injectionRate == null) throw new ArgumentNullException(nameof(injectionRate));
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            return new InjectLatencyPolicy(latency, injectionRate, enabled);
        }
    }

    public partial class MonkeyPolicy
    {
        /// <summary>
        /// Builds an <see cref="InjectLatencyPolicy{TResult}"/> which injects latency if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="latency">lambda to get the latency object</param>
        /// <param name="injectionRate">injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in context free fashion</param>
        /// <returns>The policy instance.</returns>
        [Obsolete("This overload is going to be deprecated, use the overload which takes Action<InjectLatencyOptions> instead.")]
        public static InjectLatencyPolicy<TResult> InjectLatency<TResult>(
            TimeSpan latency,
            Double injectionRate,
            Func<bool> enabled)
        {
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            return new InjectLatencyPolicy<TResult>((_, __) => latency, (_, __) => injectionRate, (_, __) => enabled());
        }

        /// <summary>
        /// Builds an <see cref="InjectLatencyPolicy{TResult}"/> which injects latency if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="latency">lambda to get the latency object</param>
        /// <param name="injectionRate">injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in current context</param>
        /// <returns>The policy instance.</returns>
        [Obsolete("This overload is going to be deprecated, use the overload which takes Action<InjectLatencyOptions> instead.")]
        public static InjectLatencyPolicy<TResult> InjectLatency<TResult>(
            TimeSpan latency,
            Double injectionRate,
            Func<Context, CancellationToken, bool> enabled)
        {
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            return new InjectLatencyPolicy<TResult>((_, __) => latency, (_, __) => injectionRate, enabled);
        }

        /// <summary>
        /// Builds an <see cref="InjectLatencyPolicy{TResult}"/> which injects latency if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="latency">lambda to get the latency object</param>
        /// <param name="injectionRate">lambda to get injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in current context</param>
        /// <returns>The policy instance.</returns>
        [Obsolete("This overload is going to be deprecated, use the overload which takes Action<InjectLatencyOptions> instead.")]
        public static InjectLatencyPolicy<TResult> InjectLatency<TResult>(
            TimeSpan latency,
            Func<Context, CancellationToken, Double> injectionRate,
            Func<Context, CancellationToken, bool> enabled)
        {
            if (injectionRate == null) throw new ArgumentNullException(nameof(injectionRate));
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            return new InjectLatencyPolicy<TResult>((_, __) => latency, injectionRate, enabled);
        }

        /// <summary>
        /// Builds an <see cref="InjectLatencyPolicy{TResult}"/> which injects latency if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="latency">lambda to get the latency object</param>
        /// <param name="injectionRate">lambda to get injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in current context</param>
        /// <returns>The policy instance.</returns>
        [Obsolete("This overload is going to be deprecated, use the overload which takes Action<InjectLatencyOptions> instead.")]
        public static InjectLatencyPolicy<TResult> InjectLatency<TResult>(
            Func<Context, CancellationToken, TimeSpan> latency,
            Func<Context, CancellationToken, Double> injectionRate,
            Func<Context, CancellationToken, bool> enabled)
        {
            if (latency == null) throw new ArgumentNullException(nameof(latency));
            if (injectionRate == null) throw new ArgumentNullException(nameof(injectionRate));
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            return new InjectLatencyPolicy<TResult>(latency, injectionRate, enabled);
        }
    }
}
