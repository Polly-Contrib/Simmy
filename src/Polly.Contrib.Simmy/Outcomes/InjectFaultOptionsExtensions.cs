using System;
using System.Threading;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Allows configuration of fault for hronous monkey fault-injection policies.
    /// </summary>
    public static class InjectFaultOptionsExtensions
    {
        /// <summary>
        /// Configure fault to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="fault">The exception to inject.</param>
        public static InjectFaultOptions<Exception> Fault(this InjectFaultOptions<Exception> options, Exception fault)
            => Fault(options, (_, __) => fault);

        /// <summary>
        /// Configure fault to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="fault">A delegate representing the fault to inject.</param>
        public static InjectFaultOptions<Exception> Fault(this InjectFaultOptions<Exception> options, Func<Context, CancellationToken, Exception> fault)
        {
            options.Outcome = fault;
            return options;
        }

        /// <summary>
        /// Configure fault to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="fault">The result to inject</param>
        public static InjectFaultOptions<Exception> Fault<TResult>(this InjectFaultOptions<Exception> options, Exception fault) =>
            Fault<TResult>(options, (_, __) => fault);

        /// <summary>
        /// Configure fault to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="fault">A delegate representing the result to inject.</param>
        public static InjectFaultOptions<Exception> Fault<TResult>(this InjectFaultOptions<Exception> options, Func<Context, CancellationToken, Exception> fault)
        {
            options.Outcome = fault;
            return options;
        }

        /// <summary>
        /// Configure result to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="result">The result to inject</param>
        public static InjectFaultOptions<TResult> Result<TResult>(this InjectFaultOptions<TResult> options, TResult result) =>
            Result(options, (_, __) => result);

        /// <summary>
        /// Configure result to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="result">A delegate representing the result to inject.</param>
        public static InjectFaultOptions<TResult> Result<TResult>(this InjectFaultOptions<TResult> options, Func<Context, CancellationToken, TResult> result)
        {
            options.Outcome = result;
            return options;
        }
    }
}
