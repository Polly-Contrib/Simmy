﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Contrib.Simmy.Outcomes
{
    /// <summary>
    /// A policy that throws an exception in place of executing the passed delegate.
    /// <remarks>The policy can also be configured to return null in place of the exception, to explicitly fake that no exception is thrown.</remarks>
    /// </summary>
    public class AsyncInjectOutcomePolicy : AsyncMonkeyPolicy
    {
        private readonly Func<Context, CancellationToken, Task<Exception>> _faultProvider;
        private readonly Func<Context, CancellationToken, Task> _beforeInjectCallback;

        [Obsolete]
        internal AsyncInjectOutcomePolicy(Func<Context, CancellationToken, Task<Exception>> faultProvider, Func<Context, CancellationToken, Task<Double>> injectionRate, Func<Context, CancellationToken, Task<bool>> enabled)
            : base(injectionRate, enabled)
        {
            _faultProvider = faultProvider ?? throw new ArgumentNullException(nameof(faultProvider));
        }
        
        internal AsyncInjectOutcomePolicy(InjectOutcomeAsyncOptions<Exception> options)
            : base(options.InjectionRate, options.Enabled)
        {
            _faultProvider = options.Outcome ?? throw new ArgumentNullException(nameof(options.Outcome));
            _beforeInjectCallback = options.BeforeInjectCallback;
        }

        /// <inheritdoc/>
        protected override Task<TResult> ImplementationAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken,
            bool continueOnCapturedContext)
        {
             return AsyncMonkeyEngine.InjectExceptionImplementationAsync(
                action,
                context,
                cancellationToken,
                _faultProvider,
                InjectionRate,
                Enabled,
                _beforeInjectCallback,
                continueOnCapturedContext);
        }
    }

    /// <summary>
    /// A policy that injects an outcome (throws an exception or returns a specific result), in place of executing the passed delegate.
    /// </summary>
    public class AsyncInjectOutcomePolicy<TResult> : AsyncMonkeyPolicy<TResult>
    {
        private readonly Func<Context, CancellationToken, Task<Exception>> _faultProvider;
        private readonly Func<Context, CancellationToken, Task<TResult>> _resultProvider;
        private readonly Func<Context, CancellationToken, Task> _beforeInjectCallback;

        [Obsolete]
        internal AsyncInjectOutcomePolicy(Func<Context, CancellationToken, Task<Exception>> faultProvider, Func<Context, CancellationToken, Task<Double>> injectionRate, Func<Context, CancellationToken, Task<bool>> enabled)
            : base(injectionRate, enabled)
        {
            _faultProvider = faultProvider ?? throw new ArgumentNullException(nameof(faultProvider));
        }

        [Obsolete]
        internal AsyncInjectOutcomePolicy(Func<Context, CancellationToken, Task<TResult>> resultProvider, Func<Context, CancellationToken, Task<Double>> injectionRate, Func<Context, CancellationToken, Task<bool>> enabled)
            : base(injectionRate, enabled)
        {
            _resultProvider = resultProvider ?? throw new ArgumentNullException(nameof(resultProvider));
        }

        internal AsyncInjectOutcomePolicy(InjectOutcomeAsyncOptions<Exception> options)
            : base(options.InjectionRate, options.Enabled)
        {
            _faultProvider = options.Outcome ?? throw new ArgumentNullException(nameof(options.Outcome));
            _beforeInjectCallback = options.BeforeInjectCallback;
        }

        internal AsyncInjectOutcomePolicy(InjectOutcomeAsyncOptions<TResult> options)
            : base(options.InjectionRate, options.Enabled)
        {
            _resultProvider = options.Outcome ?? throw new ArgumentNullException(nameof(options.Outcome));
            _beforeInjectCallback = options.BeforeInjectCallback;
        }

        /// <inheritdoc/>
        protected override async Task<TResult> ImplementationAsync(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken,
            bool continueOnCapturedContext)
        {
            if (_faultProvider != null)
            {
                return await AsyncMonkeyEngine.InjectExceptionImplementationAsync(
                    action,
                    context,
                    cancellationToken,
                    _faultProvider,
                    InjectionRate,
                    Enabled,
                    _beforeInjectCallback,
                    continueOnCapturedContext);
            }
            else if (_resultProvider != null)
            {
                return await AsyncMonkeyEngine.InjectResultImplementationAsync(
                    action,
                    context,
                    cancellationToken,
                    _resultProvider,
                    InjectionRate,
                    Enabled,
                    _beforeInjectCallback,
                    continueOnCapturedContext);
            }
            else
            {
                throw new InvalidOperationException("Either a fault or fake result to inject must be defined.");
            }
        }
    }
}
