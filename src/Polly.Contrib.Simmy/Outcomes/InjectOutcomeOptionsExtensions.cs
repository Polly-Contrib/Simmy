using System;
using System.Threading;

namespace Polly.Contrib.Simmy.Outcomes
{
    /// <summary>
    /// Allows configuration of fault for synchronous monkey fault-injection policies.
    /// </summary>
    public static class InjectOutcomeOptionsExtensions
    {
        /// <summary>
        /// Configure fault to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="fault">The exception to inject.</param>
        public static InjectOutcomeOptions<Exception> Fault(this InjectOutcomeOptions<Exception> options, Exception fault)
            => Fault(options, (_, __) => fault);

        /// <summary>
        /// Configure fault to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="fault">A delegate representing the fault to inject.</param>
        public static InjectOutcomeOptions<Exception> Fault(this InjectOutcomeOptions<Exception> options, Func<Context, CancellationToken, Exception> fault)
        {
            options.OutcomeInternal = fault;
            return options;
        }

        /// <summary>
        /// Configure fault to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="fault">The result to inject</param>
        public static InjectOutcomeOptions<Exception> Fault<TResult>(this InjectOutcomeOptions<Exception> options, Exception fault) =>
            Fault<TResult>(options, (_, __) => fault);

        /// <summary>
        /// Configure fault to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="fault">A delegate representing the result to inject.</param>
        public static InjectOutcomeOptions<Exception> Fault<TResult>(this InjectOutcomeOptions<Exception> options, Func<Context, CancellationToken, Exception> fault)
        {
            options.OutcomeInternal = fault;
            return options;
        }

        /// <summary>
        /// Configure result to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="result">The result to inject</param>
        public static InjectOutcomeOptions<TResult> Result<TResult>(this InjectOutcomeOptions<TResult> options, TResult result) =>
            Result(options, (_, __) => result);

        /// <summary>
        /// Configure result to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="result">A delegate representing the result to inject.</param>
        public static InjectOutcomeOptions<TResult> Result<TResult>(this InjectOutcomeOptions<TResult> options, Func<Context, CancellationToken, TResult> result)
        {
            options.OutcomeInternal = result;
            return options;
        }
    }
}
