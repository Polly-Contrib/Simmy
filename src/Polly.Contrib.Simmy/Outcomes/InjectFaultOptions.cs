using System;
using System.Threading;
using Polly.Contrib.Simmy.Outcomes;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Options used to configure an <see cref="InjectOutcomePolicy"/>
    /// </summary>
    public class InjectFaultOptions<TResult> : InjectOptionsBase
    {
        /// <summary>
        /// Outcome Delegate to be executed
        /// </summary>
        internal Func<Context, CancellationToken, TResult> Outcome { get; set; }
    }
}
