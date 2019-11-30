using System;
using System.Threading;

namespace Polly.Contrib.Simmy.Outcomes
{
    /// <summary>
    /// Options used to configure an <see cref="InjectOutcomePolicy"/>
    /// </summary>
    public class InjectFaultOptions<TResult> : InjectOptionsBase
    {
        /// <summary>
        /// Outcome Delegate to be executed
        /// </summary>
        internal Func<Context, CancellationToken, TResult> OutcomeInternal { get; set; }
    }
}
