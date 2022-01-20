using System;
using System.Threading;
using System.Threading.Tasks;
using Polly.Contrib.Simmy.Utilities;

namespace Polly.Contrib.Simmy
{
    internal static class AsyncMonkeyEngine
    {
        private static async Task<bool> ShouldInjectAsync(
            Context context, 
            CancellationToken cancellationToken, 
            Func<Context, CancellationToken, Task<double>> injectionRate, 
            Func<Context, CancellationToken, Task<bool>> enabled, 
            bool continueOnCapturedContext)
        {
            // to prevent execute config delegates if token is signaled before to start.
            cancellationToken.ThrowIfCancellationRequested();

            if (!await enabled(context, cancellationToken).ConfigureAwait(continueOnCapturedContext))
            {
                return false;
            }

            // to prevent execute injectionRate config delegate if token is signaled on enable configuration delegate.
            cancellationToken.ThrowIfCancellationRequested();

            double injectionThreshold = await injectionRate(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);

            // to prevent execute further config delegates if token is signaled on injectionRate configuration delegate.
            cancellationToken.ThrowIfCancellationRequested();

            injectionThreshold.EnsureInjectionThreshold();
            return ThreadSafeRandom_LockOncePerThread.NextDouble() < injectionThreshold;
        }

        internal static async Task<TResult> InjectBehaviourImplementationAsync<TResult>(
            Func<Context, CancellationToken, Task<TResult>> action, 
            Context context,
            CancellationToken cancellationToken,
            Func<Context, CancellationToken, Task> injectedBehaviour,
            Func<Context, CancellationToken, Task<Double>> injectionRate,
            Func<Context, CancellationToken, Task<bool>> enabled,
            Func<Context, CancellationToken, Task> beforeInjectCallback,
            bool continueOnCapturedContext)
        {
            if (await ShouldInjectAsync(context, cancellationToken, injectionRate, enabled, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext))
            {
                if (beforeInjectCallback != null)
                {
                    await beforeInjectCallback(context, cancellationToken);
                }
                await injectedBehaviour(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);
            }

            // to prevent execute the user's action if token is signaled on injectedBehaviour delegate.
            cancellationToken.ThrowIfCancellationRequested();
            return await action(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);
        }

        internal static async Task<TResult> InjectExceptionImplementationAsync<TResult>(
            Func<Context, CancellationToken, Task<TResult>> action,
            Context context,
            CancellationToken cancellationToken,
            Func<Context, CancellationToken, Task<Exception>> injectedException,
            Func<Context, CancellationToken, Task<Double>> injectionRate,
            Func<Context, CancellationToken, Task<bool>> enabled,
            Func<Context, CancellationToken, Task> beforeInjectCallback,
            bool continueOnCapturedContext)
        {
            if (await ShouldInjectAsync(context, cancellationToken, injectionRate, enabled, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext))
            {
                Exception exception = await injectedException(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);

                // to prevent throws the exception if token is signaled on injectedException configuration delegate.
                cancellationToken.ThrowIfCancellationRequested();

                if (exception != null)
                {
                    if (beforeInjectCallback != null)
                    {
                        await beforeInjectCallback(context, cancellationToken);
                    }
                    throw exception;
                }
            }

            return await action(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);
        }

        internal static async Task<TResult> InjectResultImplementationAsync<TResult>(
            Func<Context, CancellationToken, Task<TResult>> action,
            Context context,
            CancellationToken cancellationToken,
            Func<Context, CancellationToken, Task<TResult>> injectedResult,
            Func<Context, CancellationToken, Task<Double>> injectionRate,
            Func<Context, CancellationToken, Task<bool>> enabled,
            Func<Context, CancellationToken, Task> beforeInjectCallback,
            bool continueOnCapturedContext)
        {
            if (await ShouldInjectAsync(context, cancellationToken, injectionRate, enabled, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext))
            {
                if (beforeInjectCallback != null)
                {
                    await beforeInjectCallback(context, cancellationToken);
                }
                return await injectedResult(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);
            }

            // to prevent inject the result if token is signaled on injectedResult delegate.
            cancellationToken.ThrowIfCancellationRequested();
            return await action(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);
        }
    }
}
