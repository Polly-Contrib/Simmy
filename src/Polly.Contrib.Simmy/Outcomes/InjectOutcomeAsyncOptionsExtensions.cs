using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Contrib.Simmy.Outcomes
{
    /// <summary>
    /// Allows configuration of fault for asynchronous monkey fault-injection policies.
    /// </summary>
    public static class InjectOutcomeAsyncOptionsExtensions
    {
        /// <summary>
        /// Configure fault to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="fault">The exception to inject.</param>
        public static InjectOutcomeAsyncOptions<Exception> Fault(this InjectOutcomeAsyncOptions<Exception> options, Exception fault)
            => Fault(options, (_, __) => Task.FromResult(fault));

        /// <summary>
        /// Configure fault to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="fault">A delegate representing the fault to inject.</param>
        public static InjectOutcomeAsyncOptions<Exception> Fault(this InjectOutcomeAsyncOptions<Exception> options, Func<Context, CancellationToken, Task<Exception>> fault)
        {
            options.Outcome = fault;
            return options;
        }

        /// <summary>
        /// Configure fault to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="fault">The result to inject</param>
        public static InjectOutcomeAsyncOptions<Exception> Fault<TResult>(this InjectOutcomeAsyncOptions<Exception> options, Exception fault) =>
            Fault<TResult>(options, (_, __) => Task.FromResult(fault));

        /// <summary>
        /// Configure fault to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="fault">A delegate representing the result to inject.</param>
        public static InjectOutcomeAsyncOptions<Exception> Fault<TResult>(this InjectOutcomeAsyncOptions<Exception> options, Func<Context, CancellationToken, Task<Exception>> fault)
        {
            options.Outcome = fault;
            return options;
        }

        /// <summary>
        /// Configure result to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="result">The result to inject</param>
        public static InjectOutcomeAsyncOptions<TResult> Result<TResult>(this InjectOutcomeAsyncOptions<TResult> options, TResult result) =>
            Result(options, (_, __) => Task.FromResult(result));

        /// <summary>
        /// Configure result to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="result">A delegate representing the result to inject.</param>
        public static InjectOutcomeAsyncOptions<TResult> Result<TResult>(this InjectOutcomeAsyncOptions<TResult> options, Func<Context, CancellationToken, Task<TResult>> result)
        {
            options.Outcome = result;
            return options;
        }
    }
}
