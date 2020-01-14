using System;
using System.Threading;
using System.Threading.Tasks;
using Polly.Utilities;

namespace Polly.Contrib.Simmy.Latency
{
    /// <summary>
    /// A policy that injects latency before the execution of delegates.
    /// </summary>
    public class AsyncInjectLatencyPolicy : AsyncMonkeyPolicy
    {
        private readonly Func<Context, CancellationToken, Task<TimeSpan>> _latencyProvider;

        internal AsyncInjectLatencyPolicy(InjectLatencyAsyncOptions options)
            : base(options.InjectionRate, options.Enabled)
        {
            _latencyProvider = options.LatencyInternal ?? throw new ArgumentNullException(nameof(options.LatencyInternal));
        }

        /// <inheritdoc/>
        protected override Task<TResult> ImplementationAsync<TResult>(
            Func<Context, CancellationToken, Task<TResult>> action, 
            Context context, 
            CancellationToken cancellationToken,
            bool continueOnCapturedContext)
        {
            return AsyncMonkeyEngine.InjectBehaviourImplementationAsync(
                action,
                context,
                cancellationToken,
                async (ctx, ct) =>
                {
                    var latency = await _latencyProvider(ctx, cancellationToken).ConfigureAwait(continueOnCapturedContext);

                    // to prevent inject latency if token was signaled on latency configuration delegate.
                    cancellationToken.ThrowIfCancellationRequested();
                    await SystemClock.SleepAsync(
                            latency,
                            cancellationToken)
                        .ConfigureAwait(continueOnCapturedContext);
                },
                InjectionRate,
                Enabled,
                continueOnCapturedContext);
        }
    }

    /// <summary>
    /// A policy that injects latency before the execution of delegates.
    /// </summary>
    public class AsyncInjectLatencyPolicy<TResult> : AsyncMonkeyPolicy<TResult>
    {
        private readonly Func<Context, CancellationToken, Task<TimeSpan>> _latencyProvider;

        internal AsyncInjectLatencyPolicy(InjectLatencyAsyncOptions options)
            : base(options.InjectionRate, options.Enabled)
        {
            _latencyProvider = options.LatencyInternal ?? throw new ArgumentNullException(nameof(options.LatencyInternal));
        }

        /// <inheritdoc/>
        protected override Task<TResult> ImplementationAsync(
            Func<Context, CancellationToken, Task<TResult>> action,
            Context context,
            CancellationToken cancellationToken,
            bool continueOnCapturedContext)
        {
            return AsyncMonkeyEngine.InjectBehaviourImplementationAsync(
                action,
                context,
                cancellationToken,
                async (ctx, ct) =>
                {
                    var latency = await _latencyProvider(ctx, cancellationToken).ConfigureAwait(continueOnCapturedContext);

                    // to prevent inject latency if token was signaled on latency configuration delegate.
                    cancellationToken.ThrowIfCancellationRequested();
                    await SystemClock.SleepAsync(
                            latency,
                            cancellationToken)
                        .ConfigureAwait(continueOnCapturedContext);
                },
                InjectionRate,
                Enabled,
                continueOnCapturedContext);
        }
    }
}
