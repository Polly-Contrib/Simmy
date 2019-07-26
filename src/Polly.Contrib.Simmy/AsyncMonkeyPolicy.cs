using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Contains common functionality for policies which intentionally disrupt async executions - which monkey around with calls.
    /// </summary>
    public abstract class AsyncMonkeyPolicy : AsyncPolicy, IMonkeyPolicy
    {
        internal Func<Context, CancellationToken, Task<Double>> InjectionRate { get; }
        internal Func<Context, CancellationToken, Task<bool>> Enabled { get; }

        internal AsyncMonkeyPolicy(Func<Context, CancellationToken, Task<Double>> injectionRate, Func<Context, CancellationToken, Task<bool>> enabled)
        {
            InjectionRate = injectionRate ?? throw new ArgumentNullException(nameof(injectionRate));
            Enabled = enabled ?? throw new ArgumentNullException(nameof(enabled));
        }
    }

    /// <summary>
    /// Contains common functionality for policies which intentionally disrupt async executions returning TResult - which monkey around with calls.
    /// </summary>
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    public abstract class AsyncMonkeyPolicy<TResult> : AsyncPolicy<TResult>, IMonkeyPolicy<TResult>
    {
        internal Func<Context, CancellationToken, Task<Double>> InjectionRate { get; }
        internal Func<Context, CancellationToken, Task<bool>> Enabled { get; }

        internal AsyncMonkeyPolicy(Func<Context, CancellationToken, Task<Double>> injectionRate, Func<Context, CancellationToken, Task<bool>> enabled) 
        {
            InjectionRate = injectionRate ?? throw new ArgumentNullException(nameof(injectionRate));
            Enabled = enabled ?? throw new ArgumentNullException(nameof(enabled));
        }
    }
}
