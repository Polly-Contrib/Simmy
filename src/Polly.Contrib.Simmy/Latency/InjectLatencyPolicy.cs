using System;
using System.Threading;
using Polly.Utilities;

namespace Polly.Contrib.Simmy.Latency
{
    /// <summary>
    /// A policy that injects latency before the execution of delegates.
    /// </summary>
    public class InjectLatencyPolicy : MonkeyPolicy
    {
        private readonly Func<Context, CancellationToken, TimeSpan> _latencyProvider;

        [Obsolete]
        internal InjectLatencyPolicy(
            Func<Context, CancellationToken, TimeSpan> latencyProvider,
            Func<Context, CancellationToken, double> injectionRate,
            Func<Context, CancellationToken, bool> enabled)
            : base(injectionRate, enabled)
        {
            _latencyProvider = latencyProvider ?? throw new ArgumentNullException(nameof(latencyProvider));
        }

        internal InjectLatencyPolicy(InjectLatencyOptions options)
            : base(options.InjectionRate, options.Enabled)
        {
            _latencyProvider = options.Latency ?? throw new ArgumentNullException(nameof(options.Latency));
        }

        /// <inheritdoc/>
        protected override TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
        {
            return MonkeyEngine.InjectBehaviourImplementation(
                action,
                context,
                cancellationToken,
                (ctx, ct) =>
                {
                    var latency = _latencyProvider(ctx, ct);

                    // to prevent inject latency if token was signaled on latency configuration delegate.
                    cancellationToken.ThrowIfCancellationRequested();
                    SystemClock.Sleep(latency, cancellationToken);
                },
                InjectionRate,
                Enabled);
        }
    }

    /// <summary>
    /// A policy that injects latency before the execution of delegates.
    /// </summary>
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    public class InjectLatencyPolicy<TResult> : MonkeyPolicy<TResult>
    {
        private readonly Func<Context, CancellationToken, TimeSpan> _latencyProvider;

        [Obsolete]
        internal InjectLatencyPolicy(
            Func<Context, CancellationToken, TimeSpan> latencyProvider,
            Func<Context, CancellationToken, double> injectionRate,
            Func<Context, CancellationToken, bool> enabled)
            : base(injectionRate, enabled)
        {
            _latencyProvider = latencyProvider ?? throw new ArgumentNullException(nameof(latencyProvider));
        }

        internal InjectLatencyPolicy(InjectLatencyOptions options)
            : base(options.InjectionRate, options.Enabled)
        {
            _latencyProvider = options.Latency ?? throw new ArgumentNullException(nameof(options.Latency));
        }

        /// <inheritdoc/>
        protected override TResult Implementation(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
        {
            return MonkeyEngine.InjectBehaviourImplementation(
                action,
                context,
                cancellationToken,
                (ctx, ct) =>
                {
                    var latency = _latencyProvider(ctx, ct);

                    // to prevent inject latency if token was signaled on latency configuration delegate.
                    cancellationToken.ThrowIfCancellationRequested();
                    SystemClock.Sleep(latency, cancellationToken);
                },
                InjectionRate,
                Enabled);
        }
    }
}
