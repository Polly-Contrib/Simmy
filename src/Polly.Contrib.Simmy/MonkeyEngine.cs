using System;
using System.Threading;
using Polly.Contrib.Simmy.Utilities;

namespace Polly.Contrib.Simmy
{
    internal static class MonkeyEngine
    {
        private static bool ShouldInject(Context context, CancellationToken cancellationToken, Func<Context, CancellationToken, double> injectionRate, Func<Context, CancellationToken, bool> enabled)
        {
            // to prevent execute config delegates if token is signaled before to start.
            cancellationToken.ThrowIfCancellationRequested();

            if (!enabled(context, cancellationToken))
            {
                return false;
            }

            // to prevent execute injectionRate config delegate if token is signaled on enable configuration delegate.
            cancellationToken.ThrowIfCancellationRequested();

            double injectionThreshold = injectionRate(context, cancellationToken);

            // to prevent execute further config delegates if token is signaled on injectionRate configuration delegate.
            cancellationToken.ThrowIfCancellationRequested();

            injectionThreshold.EnsureInjectionThreshold();
            return ThreadSafeRandom_LockOncePerThread.NextDouble() < injectionThreshold;
        }

        internal static TResult InjectBehaviourImplementation<TResult>(
            Func<Context, CancellationToken, TResult> action,
            Context context,
            CancellationToken cancellationToken,
            Action<Context, CancellationToken> injectedBehaviour,
            Func<Context, CancellationToken, Double> injectionRate,
            Func<Context, CancellationToken, bool> enabled,
            Action<Context, CancellationToken> beforeInjectCallback)
        {
            if (ShouldInject(context, cancellationToken, injectionRate, enabled))
            {
                if (beforeInjectCallback != null)
                {
                    beforeInjectCallback(context, cancellationToken);
                }
                injectedBehaviour(context, cancellationToken);
            }

            // to prevent execute the user's action if token is signaled on injectedBehaviour delegate.
            cancellationToken.ThrowIfCancellationRequested();
            return action(context, cancellationToken);
        }

        internal static TResult InjectExceptionImplementation<TResult>(
            Func<Context, CancellationToken, TResult> action,
            Context context,
            CancellationToken cancellationToken,
            Func<Context, CancellationToken, Exception> injectedException,
            Func<Context, CancellationToken, Double> injectionRate,
            Func<Context, CancellationToken, bool> enabled,
            Action<Context, CancellationToken> beforeInjectCallback)
        {
            if (ShouldInject(context, cancellationToken, injectionRate, enabled))
            {
                Exception exception = injectedException(context, cancellationToken);

                // to prevent throws the exception if token is signaled on injectedException configuration delegate.
                cancellationToken.ThrowIfCancellationRequested();

                if (exception != null)
                {
                    if (beforeInjectCallback != null)
                    {
                        beforeInjectCallback(context, cancellationToken);
                    }
                    throw exception;
                }
            }

            return action(context, cancellationToken);
        }

        internal static TResult InjectResultImplementation<TResult>(
            Func<Context, CancellationToken, TResult> action,
            Context context,
            CancellationToken cancellationToken,
            Func<Context, CancellationToken, TResult> injectedResult,
            Func<Context, CancellationToken, Double> injectionRate,
            Func<Context, CancellationToken, bool> enabled,
            Action<Context, CancellationToken> beforeInjectCallback)
        {
            if (ShouldInject(context, cancellationToken, injectionRate, enabled))
            {
                if (beforeInjectCallback != null)
                {
                    beforeInjectCallback(context, cancellationToken);
                }
                return injectedResult(context, cancellationToken);
            }

            // to prevent inject the result if token is signaled on injectedResult delegate.
            cancellationToken.ThrowIfCancellationRequested();
            return action(context, cancellationToken);
        }
    }
}
