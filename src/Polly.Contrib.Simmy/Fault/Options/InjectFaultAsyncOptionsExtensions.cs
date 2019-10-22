using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Contrib.Simmy.Fault.Options
{
    /// <summary>
    /// Allows configuration of fault for asynchronous monkey fault-injection policies.
    /// </summary>
    public static class InjectFaultAsyncOptionsExtensions
    {
        /// <summary>
        /// Configure fault to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="fault">The exception to inject.</param>
        public static InjectFaultAsyncOptions<Exception> Fault(this InjectFaultAsyncOptions<Exception> options, Exception fault)
            => Fault(options, (_, __) => Task.FromResult(fault));

        /// <summary>
        /// Configure fault to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="fault">A delegate representing the fault to inject.</param>
        public static InjectFaultAsyncOptions<Exception> Fault(this InjectFaultAsyncOptions<Exception> options, Func<Context, CancellationToken, Task<Exception>> fault)
        {
            options.Outcome = fault;
            return options;
        }

        /// <summary>
        /// Configure fault to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="fault">The result to inject</param>
        public static InjectFaultAsyncOptions<Exception> Fault<TResult>(this InjectFaultAsyncOptions<Exception> options, Exception fault) =>
            Fault<TResult>(options, (_, __) => Task.FromResult(fault));

        /// <summary>
        /// Configure fault to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="fault">A delegate representing the result to inject.</param>
        public static InjectFaultAsyncOptions<Exception> Fault<TResult>(this InjectFaultAsyncOptions<Exception> options, Func<Context, CancellationToken, Task<Exception>> fault)
        {
            options.Outcome = fault;
            return options;
        }

        /// <summary>
        /// Configure fault to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="result">The result to inject</param>
        public static InjectFaultAsyncOptions<TResult> Result<TResult>(this InjectFaultAsyncOptions<TResult> options, TResult result) =>
            Result(options, (_, __) => Task.FromResult(result));

        /// <summary>
        /// Configure fault to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="result">A delegate representing the result to inject.</param>
        public static InjectFaultAsyncOptions<TResult> Result<TResult>(this InjectFaultAsyncOptions<TResult> options, Func<Context, CancellationToken, Task<TResult>> result)
        {
            options.Outcome = result;
            return options;
        }
    }
}
