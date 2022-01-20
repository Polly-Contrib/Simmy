﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Contrib.Simmy.Behavior
{
    /// <summary>
    /// A policy that injects any custom behaviour before the execution of delegates.
    /// </summary>
    public class AsyncInjectBehaviourPolicy : AsyncMonkeyPolicy
    {
        private readonly Func<Context, CancellationToken, Task> _behaviour;
        private readonly Func<Context, CancellationToken, Task> _beforeInjectCallback;

        [Obsolete]
        internal AsyncInjectBehaviourPolicy(Func<Context, CancellationToken, Task> behaviour, Func<Context, CancellationToken, Task<Double>> injectionRate, Func<Context, CancellationToken, Task<bool>> enabled)
            : base(injectionRate, enabled)
        {
            _behaviour = behaviour ?? throw new ArgumentNullException(nameof(behaviour));
        }

        internal AsyncInjectBehaviourPolicy(InjectBehaviourAsyncOptions options)
            : base(options.InjectionRate, options.Enabled)
        {
            _behaviour = options.BehaviourInternal ?? throw new ArgumentNullException(nameof(options.BehaviourInternal));
            _beforeInjectCallback = options.BeforeInjectCallback;
        }

        /// <inheritdoc/>
        protected override Task<TResult> ImplementationAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken,
            bool continueOnCapturedContext)
        {
            return AsyncMonkeyEngine.InjectBehaviourImplementationAsync(
                action,
                context,
                cancellationToken,
                _behaviour,
                InjectionRate,
                Enabled,
                _beforeInjectCallback,
                continueOnCapturedContext);
        }
    }

    /// <summary>
    /// A policy that injects any custom behaviour before the execution of delegates returning <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    public class AsyncInjectBehaviourPolicy<TResult> : AsyncMonkeyPolicy<TResult>
    {
        private readonly Func<Context, CancellationToken, Task> _behaviour;
        private readonly Func<Context, CancellationToken, Task> _beforeInjectCallback;

        [Obsolete]
        internal AsyncInjectBehaviourPolicy(Func<Context, CancellationToken, Task> behaviour, Func<Context, CancellationToken, Task<Double>> injectionRate, Func<Context, CancellationToken, Task<bool>> enabled)
            : base(injectionRate, enabled)
        {
            _behaviour = behaviour ?? throw new ArgumentNullException(nameof(behaviour));
        }

        internal AsyncInjectBehaviourPolicy(InjectBehaviourAsyncOptions options)
            : base(options.InjectionRate, options.Enabled)
        {
            _behaviour = options.BehaviourInternal ?? throw new ArgumentNullException(nameof(options.BehaviourInternal));
            _beforeInjectCallback = options.BeforeInjectCallback;
        }

        /// <inheritdoc/>
        protected override Task<TResult> ImplementationAsync(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            return AsyncMonkeyEngine.InjectBehaviourImplementationAsync(
                action,
                context,
                cancellationToken,
                _behaviour,
                InjectionRate,
                Enabled,
                _beforeInjectCallback,
                continueOnCapturedContext);
        }
    }
}
